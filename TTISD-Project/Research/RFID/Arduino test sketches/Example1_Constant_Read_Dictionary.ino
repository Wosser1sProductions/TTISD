#include <ArduinoSTL.h>

/*
    Reading multiple RFID tags, simultaneously!
    By: Nathan Seidle @ SparkFun Electronics
    Date: October 3rd, 2016
    https://github.com/sparkfun/Simultaneous_RFID_Tag_Reader

    Constantly reads and outputs any tags heard

    If using the Simultaneous RFID Tag Reader (SRTR) shield, make sure the serial slide
    switch is in the 'SW-UART' position
*/

#include <SoftwareSerial.h> //Used for transmitting to the device

SoftwareSerial softSerial(2, 3); //RX, TX

#include "SparkFun_UHF_RFID_Reader.h" //Library for controlling the M6E Nano module
#include <map>
RFID nano; //Create instance

void setup() {
    Serial.begin(115200);

    while (!Serial)
        ; //Wait for the serial port to come online

    if (setupNano(38400) == false) { //Configure nano to run at 38400bps
        Serial.println(F("Module failed to respond. Please check wiring."));

        while (1)
            ; //Freeze!
    }

    nano.setRegion(REGION_EUROPE); //Set to North America

    nano.setReadPower(1500); //5.00 dBm. Higher values may caues USB port to brown out
    //Max Read TX Power is 27.00 dBm and may cause temperature-limit throttling

    Serial.println(F("Press a key to begin scanning for tags."));

    while (!Serial.available())
        ;            //Wait for user to send a character

    Serial.read(); //Throw away the user's character

    nano.startReading(); //Begin scanning for tags
}

std::map<String, int> epcs;
#define WAIT_MS 3000
int stamp, prev = 0;

void loop() {
    stamp = millis();

    if (nano.check() == true) { //Check to see if any new data has come in from module
        byte responseType = nano.parseResponse(); //Break response into tag ID, RSSI, frequency, and timestamp

        if (responseType == RESPONSE_IS_KEEPALIVE) {
            Serial.println(F("Scanning"));
        } else if (responseType == RESPONSE_IS_TAGFOUND) {
            byte tagEPCBytes = nano.getTagEPCBytes(); //Get the number of bytes of EPC from response

            String stringOne = "";

            for (byte x = 0; x < tagEPCBytes; x++) {
                if (nano.msg[31 + x] < 0x10)
                    stringOne = String(stringOne + F("0")); //Pretty print

                stringOne = String(stringOne + String(nano.msg[31 + x], HEX));
                stringOne = String(stringOne + F(" "));
            }

            Serial.print(stringOne);
            epcs[stringOne] = stamp;

            Serial.print(" ==> ");
            Serial.println(epcs.size());
        } else if (responseType == ERROR_CORRUPT_RESPONSE) {
            Serial.println("Bad CRC");
        } else {
            //Unknown response
            Serial.print("Unknown error");
        }
    }

    for (std::map<String, int>::value_type& x : epcs) {
        if (stamp - x.second > WAIT_MS) {
            epcs.erase(x.first);
        }
    }

    delay(10);
}

//Gracefully handles a reader that is already configured and already reading continuously
//Because Stream does not have a .begin() we have to do this outside the library
boolean setupNano(long baudRate) {
    nano.begin(softSerial); //Tell the library to communicate over software serial port

    //Test to see if we are already connected to a module
    //This would be the case if the Arduino has been reprogrammed and the module has stayed powered
    softSerial.begin(baudRate); //For this test, assume module is already at our desired baud rate

    while (softSerial.isListening() == false)
        ; //Wait for port to open

    //About 200ms from power on the module will send its firmware version at 115200. We need to ignore this.
    while (softSerial.available())
        softSerial.read();

    nano.getVersion();

    if (nano.msg[0] == ERROR_WRONG_OPCODE_RESPONSE) {
        //This happens if the baud rate is correct but the module is doing a ccontinuous read
        nano.stopReading();

        Serial.println(F("Module continuously reading. Asking it to stop..."));

        delay(1500);
    } else {
        //The module did not respond so assume it's just been powered on and communicating at 115200bps
        softSerial.begin(115200); //Start software serial at 115200

        nano.setBaud(baudRate); //Tell the module to go to the chosen baud rate. Ignore the response msg

        softSerial.begin(baudRate); //Start the software serial port, this time at user's chosen baud rate

        delay(250);
    }

    //Test the connection
    nano.getVersion();

    if (nano.msg[0] != ALL_GOOD)
        return (false); //Something is not right

    //The M6E has these settings no matter what
    nano.setTagProtocol(); //Set protocol to GEN2

    nano.setAntennaPort(); //Set TX/RX antenna ports to 1

    return (true); //We are ready to rock
}

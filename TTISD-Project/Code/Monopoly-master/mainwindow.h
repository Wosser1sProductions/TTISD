#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QtGui>
#include <QMainWindow>
#include "centralwidget.h"
#include "bottomBar.h"
#include "guiplayers.h"
#include "Game_Board.h"
#include <QPlainTextEdit>
#include <QShortcut>
#include "player.h"
#include "space.h"

#include "RPLIDAR_wrapper.h"


class MainWindow : public QMainWindow {
        Q_OBJECT

    private:
        GUIPlayers *guiPlayers[4];
        CentralWidget *centralWidget;
        BottomBar *bottomBar;
        int playerTurn;
        int totalPlayers;
        Player *players;
        Coordinates boardSpace[40];
        Space **spaces;

        virtual void keyPressEvent(QKeyEvent *event);

        lidar::Driver rplidardriver;
        std::vector<bool> prev_player_state;

    public:
        MainWindow(int numPlayers, lidar::Settings lidar_settings);
        //    MainWindow();
        void nextTurn();
        int getPlayerTurn();
        void setGamePiece(int playerNum, string pieceName);
        void windowSetUp();
        string getGamePiece(int playerNum);
        void setPlayerLocation(int playerNum, int spaceNum);
        int getPlayerLocation(int playerNum);
        Coordinates getPlayerPixels(int playerNum);
        int getSpaceOwnership(int spaceIndex);
        string spaceType(int inputIndex);
        Player *getPlayer(int num);
        string getPlayerName(int playerNum);
        int getSpaceRent(int spaceIndex);
        int getSpaceUpgradedAmount(int spaceIndex);
        int getPlayerMoney(int playerNum);
        int getSpacePropertyCost(int spaceIndex);
        void setSpaceOwnership(int spaceIndex, int playerNum);
        void playerLost(int playerNum);
        bool isPlayerAlive(int playerNum);
        int getSpaceTax(int spaceIndex);
        string getSpaceName(int spaceIndex, int index);
        Space **getAllSpaces();
        int getTotalPlayers();
        void changeLidarSettings(lidar::Settings cfg);
        lidar::Settings getLidarSettings() const;
        bool isLidarAvailable() const;

        lidar::PlayerMovement getPlayerPositionDiff();
        void movedPiece(int currentPos, int newPos);
};

#endif

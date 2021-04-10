using Robocode;
using Robocode.Util;
using System;
using System.Drawing;


namespace CAP4053.Student
{
    public class BellaBot : TeamRobot
    {
        private int turnCount;
        private string enemyBot;
        private double enemyDistance;
        private double enemyX;
        private double predictEnemyX;
        private double predictEnemyY;
        private double enemyY;
        private double radarAngle;
        private double scanDirection = 1.0;
        private double moveDirection = 1.0;
        private double firepower;
        private double bulletVel;
        private double gunAngle;

        private class Enemy
        {
            public string enemyName;
            public double enemyDistance;
            public double xLoc;
            public double yLoc;
        }

        private Enemy enemy = new Enemy();
        
        public override void Run()
        {
            //Initialize Colors
            BodyColor = (Color.Purple);
            GunColor = (Color.Purple);
            RadarColor = (Color.Purple);
            ScanColor = (Color.Aquamarine);
            BulletColor = (Color.LimeGreen);

            //isolate all robot movement
            IsAdjustGunForRobotTurn = true; //isolate gun turn
            IsAdjustRadarForGunTurn = true; //isolate radar turn from gun
            IsAdjustRadarForRobotTurn = true; //isolate radar turn from robot

            enemyBot = null;
            turnCount = 0;
            TurnRadarRight(360);
            while (true)
            {
                //never stop moving radar to ensure scanning continues
                if (RadarTurnRemaining == 0)
                {
                    SetTurnRadarRight((radarAngle + 2));
                    setGunAngle();
                }
                else
                {
                    SetTurnRadarRight(radarAngle * scanDirection);
                    setGunAngle();
                }
                Execute();
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            //if no bot is being tracked, assign tracker to this new scan
            if (enemyBot == null)
            {
                enemyBot = evnt.Name;
                trackEm(evnt);
                Out.WriteLine("Tracking " + enemyBot);
            }

            //if bot is already being tracked but finds another bot closer, start tracking this bot or continue tracking current
            if (enemyBot != null || evnt.Distance < enemyDistance || evnt.Name.Equals(enemyBot))
            {
                enemyBot = evnt.Name;
                enemyDistance = evnt.Distance;
                trackEm(evnt);
            }

            //reverse scan direction if bearing is negative
            if (evnt.Bearing < 0)
            {
                scanDirection = scanDirection * -1.0;
            }

            SetTurnRight(Utils.NormalRelativeAngleDegrees(evnt.Bearing));
            radarAngle = Utils.NormalRelativeAngleDegrees((Heading + evnt.Bearing) - RadarHeading);
            SetTurnRadarRight(Utils.NormalRelativeAngleDegrees((Heading + evnt.Bearing) - RadarHeading));
            Out.WriteLine("Enemy X: " + enemyX);
            Out.WriteLine("Enemy Y: " + enemyY);
            Out.WriteLine("Bearing Read: " + evnt.Bearing);
            Out.WriteLine("Predicted X: " + predictEnemyX);
            Out.WriteLine("Predicted Y: " + predictEnemyY);
            SetAhead(evnt.Distance);
            moveDirection = 1.0;
            if (evnt.Distance < 300)
            {
                if (X < (BattleFieldWidth - BattleFieldWidth + 100))
                {
                    if (moveDirection == -1.0)
                    {
                        SetAhead(100);
                        moveDirection = 1.0;
                    }
                    else
                    {
                        SetBack(100);
                        moveDirection = -1.0;
                    }
                }
                else if (X > (BattleFieldWidth - 100))
                {
                    if (moveDirection == -1.0)
                    {
                        SetAhead(100);
                        moveDirection = 1.0;
                    }
                    else
                    {
                        SetBack(100);
                        moveDirection = -1.0;
                    }
                }
                else if (Y < (BattleFieldHeight - BattleFieldHeight + 100))
                {
                    if (moveDirection == -1.0)
                    {
                        SetAhead(100);
                        moveDirection = 1.0;
                    }
                    else
                    {
                        SetBack(100);
                        moveDirection = -1.0;
                    }
                }
                else if (Y > BattleFieldHeight - 100)
                {
                    if (moveDirection == -1.0)
                    {
                        SetAhead(100);
                        moveDirection = 1.0;
                    }
                    else
                    {
                        SetBack(100);
                        moveDirection = -1.0;
                    }
                }
                SetTurnRight(Utils.NormalRelativeAngleDegrees(evnt.Bearing + 80));
            }
        }

        public void trackEm(ScannedRobotEvent scan)
        {
            double bearingDeg = Heading + scan.Bearing;
            if (bearingDeg < 0)
            {
                bearingDeg += 360;
            }
            //get current coordinate position
            enemyX = X + Math.Sin(deg2Rad(bearingDeg)) * scan.Distance;
            enemyY = Y + Math.Cos(deg2Rad(bearingDeg)) * scan.Distance;

            //determine firepower based on distance
            //robocde bullet velocity = 20 - 3 * firepower
            firepower = Math.Min(300 / scan.Distance, 3);
            bulletVel = 20 - (3 * firepower);

            //predict enemy location based on distance, enemy direction, enemy speed, and bullet velocity
            predictEnemyX = enemyX + Math.Sin(deg2Rad(scan.Heading)) * scan.Velocity * (scan.Distance / (bulletVel - Velocity));
            predictEnemyY = enemyY + Math.Cos(deg2Rad(scan.Heading)) * scan.Velocity * (scan.Distance / (bulletVel - Velocity));
        }

        public double deg2Rad(double theta)
        {
            return ((theta * Math.PI) / 180);
        }

        public double rad2Deg(double theta)
        {
            return ((theta * 180) / Math.PI);
        }

        public void setGunAngle()
        {
            double x = predictEnemyX - X;
            double y = predictEnemyY - Y;

            double xSq = x * x;
            double ySq = y * y;
            double distance = Math.Sqrt(xSq + ySq);
            double theta = rad2Deg(Math.Asin(x / distance));
            gunAngle = 0;

            if (x > 0 && y > 0)
            {
                gunAngle = theta;
            }
            else if (x < 0 && y > 0)
            {
                gunAngle = theta + 360;
            }
            else if (x > 0 && y < 0)
            {
                gunAngle = 180 - theta;
            }
            else if (x < 0 && y < 0)
            {
                gunAngle = 180 - theta;
            }

            SetTurnGunRight(Utils.NormalRelativeAngleDegrees(gunAngle - GunHeading));
            if (GunHeat == 0 && GunTurnRemaining < 10)
            {
                SetFire(firepower);
            }
        }
    }
}

﻿using Door_of_Soul.Communication.HexagramNodeServer;
using Door_of_Soul.Core;
using Door_of_Soul.Server;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using Photon.SocketServer;
using System.IO;


namespace Door_of_Soul.HexagramDestinyServer.PhotonServer
{
    class HexagramDestinyServerEnvironment : ServerEnvironment.ServerEnvironment
    {
        public static CentralPeer CentralPeer { get; private set; }
        public static bool ConnectHexagrameCentralServer(out string errorMessage)
        {
            if (CentralCommunicationService.Instance.ConnectHexagrameCentralServer(
                serverAddress: ServerEnvironmentConfiguration.Instance.HexagramCentralServerAddress,
                port: ServerEnvironmentConfiguration.Instance.HexagramCentralServerPort,
                applicationName: ServerEnvironmentConfiguration.Instance.HexagramCentralServerApplicationName))
            {
                errorMessage = "";
                return true;
            }
            else
            {
                errorMessage = "ConnectHexagrameCentralServer Failed";
                return false;
            }
        }

        public override bool SetupCommunication(out string errorMessage)
        {
            CentralCommunicationService.Initialize(new HexagramCentralCommunicationService());
            CentralPeer = new CentralPeer(ApplicationBase.Instance);
            return ConnectHexagrameCentralServer(out errorMessage);
        }

        public override bool SetupConfiguration(out string errorMessage)
        {
            ServerEnvironmentConfiguration serverEnvironmentConfiguration;
            if (GenericConfigurationLoader<ServerEnvironmentConfiguration>.Load(Path.Combine(ApplicationBase.Instance.ApplicationPath, "config", "ServerEnvironment.config"), out serverEnvironmentConfiguration))
            {
                ServerEnvironmentConfiguration.Initialize(serverEnvironmentConfiguration);
                errorMessage = "";
                return true;
            }
            else
            {
                errorMessage = "ServerEnvironmentConfiguration Load Failed";
                return false;
            }
        }

        public override bool SetupLog(out string errorMessage)
        {
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(ApplicationBase.Instance.ApplicationPath, "log");
            FileInfo file = new FileInfo(Path.Combine(ApplicationBase.Instance.BinaryPath, "log4net.config"));
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }
            else
            {
                errorMessage = "Setup Log Failed";
                return false;
            }

            DestinyHexagramEntranceFactory.Instance.OnSubjectAdded += LogEntranceConnected;
            DestinyHexagramEntranceFactory.Instance.OnSubjectRemoved += LogEntranceDisconnected;

            errorMessage = "";
            return true;
        }

        public override bool SetupServer(out string errorMessage)
        {
            ServerInitializer.Initialize(new HexagramDestinyServerInitializer());
            return ServerInitializer.Instance.Initialize(out errorMessage);
        }

        public override void TearDown()
        {
            DestinyHexagramEntranceFactory.Instance.OnSubjectAdded -= LogEntranceConnected;
            DestinyHexagramEntranceFactory.Instance.OnSubjectRemoved -= LogEntranceDisconnected;
        }

        private void LogEntranceDisconnected(DestinyHexagramEntrance entrance)
        {
            HexagramDestinyServerApplication.Log.Info($"Entrance: {entrance} disconnected");
        }

        private void LogEntranceConnected(DestinyHexagramEntrance entrance)
        {
            HexagramDestinyServerApplication.Log.Info($"Entrance: {entrance} connected");
        }

        public override bool SetupDatabase(out string errorMessage)
        {
            errorMessage = "";
            return true;
        }
    }
}

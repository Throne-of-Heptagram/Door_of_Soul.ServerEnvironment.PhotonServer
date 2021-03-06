﻿using Door_of_Soul.Communication.HexagramEntranceServer;
using Door_of_Soul.Core;
using Door_of_Soul.Database.Connection;
using Door_of_Soul.Database.MariaDb.Connection;
using Door_of_Soul.Database.MariaDb.Repository.Throne;
using Door_of_Soul.Database.Repository.Throne;
using Door_of_Soul.Server;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using MySql.Data.MySqlClient;
using Photon.SocketServer;
using System.Collections.Generic;
using System.IO;

namespace Door_of_Soul.HexagramEntranceServer.PhotonServer
{
    class HexagramEntranceServerEnvironment : ServerEnvironment.ServerEnvironment
    {
        public static KnowledgePeer KnowledgePeer { get; private set; }
        public static LifePeer LifePeer { get; private set; }
        public static ElementPeer ElementPeer { get; private set; }
        public static InfinitePeer InfinitePeer { get; private set; }
        public static LovePeer LovePeer { get; private set; }
        public static SpacePeer SpacePeer { get; private set; }
        public static WillPeer WillPeer { get; private set; }
        public static ShadowPeer ShadowPeer { get; private set; }
        public static HistoryPeer HistoryPeer { get; private set; }
        public static EternityPeer EternityPeer { get; private set; }
        public static DestinyPeer DestinyPeer { get; private set; }
        public static ThronePeer ThronePeer { get; private set; }

        private static Dictionary<HexagramNodeServerType, HexagramCommunicationService> nodeServiceDictionary = new Dictionary<HexagramNodeServerType, HexagramCommunicationService>();

        public static bool ConnectHexagrameNodeServer(HexagramNodeServerType nodeServerType, out string errorMessage)
        {
            int nodeServerIndex = (int)nodeServerType;
            if (nodeServiceDictionary[nodeServerType].ConnectServer(
                hexagramEntranceId: ServerEnvironmentConfiguration.Instance.HexagramEntranceId,
                serverAddress: ServerEnvironmentConfiguration.Instance.HexagramNodeServerAddresses[nodeServerIndex],
                port: ServerEnvironmentConfiguration.Instance.HexagramNodeServerPorts[nodeServerIndex],
                applicationName: ServerEnvironmentConfiguration.Instance.HexagramNodeServerApplicationNames[nodeServerIndex]))
            {
                errorMessage = "";
                return true;
            }
            else
            {
                errorMessage = $"ConnectHexagrame{nodeServerType}Server Failed";
                return false;
            }
        }

        public override bool SetupCommunication(out string errorMessage)
        {
            CommunicationService.Initialize(new HexagramEntranceServerCommunicationService());
            KnowledgeCommunicationService.Initialize(new HexagramEntranceServerKnowledgeCommunicationService());
            LifeCommunicationService.Initialize(new HexagramEntranceServerLifeCommunicationService());
            ElementCommunicationService.Initialize(new HexagramEntranceServeElementCommunicationService());
            InfiniteCommunicationService.Initialize(new HexagramEntranceServerInfiniteCommunicationService());
            LoveCommunicationService.Initialize(new HexagramEntranceServerLoveCommunicationService());
            SpaceCommunicationService.Initialize(new HexagramEntranceServerSpaceCommunicationService());
            WillCommunicationService.Initialize(new HexagramEntranceServeWillCommunicationService());
            ShadowCommunicationService.Initialize(new HexagramEntranceServeShadowCommunicationService());
            HistoryCommunicationService.Initialize(new HexagramEntranceServerHistoryCommunicationService());
            EternityCommunicationService.Initialize(new HexagramEntranceServerEternityCommunicationService());
            DestinyCommunicationService.Initialize(new HexagramEntranceServerDestinyCommunicationService());
            ThroneCommunicationService.Initialize(new HexagramEntranceServerThroneCommunicationService());

            KnowledgePeer = new KnowledgePeer(ApplicationBase.Instance);
            LifePeer = new LifePeer(ApplicationBase.Instance);
            ElementPeer = new ElementPeer(ApplicationBase.Instance);
            InfinitePeer = new InfinitePeer(ApplicationBase.Instance);
            LovePeer = new LovePeer(ApplicationBase.Instance);
            SpacePeer = new SpacePeer(ApplicationBase.Instance);
            WillPeer = new WillPeer(ApplicationBase.Instance);
            ShadowPeer = new ShadowPeer(ApplicationBase.Instance);
            HistoryPeer = new HistoryPeer(ApplicationBase.Instance);
            EternityPeer = new EternityPeer(ApplicationBase.Instance);
            DestinyPeer = new DestinyPeer(ApplicationBase.Instance);
            ThronePeer = new ThronePeer(ApplicationBase.Instance);

            nodeServiceDictionary.Add(HexagramNodeServerType.Knowledge, KnowledgeCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Life, LifeCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Element, ElementCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Infinite, InfiniteCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Love, LoveCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Space, SpaceCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Will, WillCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Shadow, ShadowCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.History, HistoryCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Eternity, EternityCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Destiny, DestinyCommunicationService.Instance);
            nodeServiceDictionary.Add(HexagramNodeServerType.Throne, ThroneCommunicationService.Instance);

            foreach(var pair in nodeServiceDictionary)
            {
                if (!ConnectHexagrameNodeServer(pair.Key, out errorMessage))
                {
                    return false;
                }
            }
            errorMessage = "";
            return true;
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

            EndPointFactory.Instance.OnSubjectAdded += LogEndPointConnected;
            EndPointFactory.Instance.OnSubjectRemoved += LogEndPointDisconnected;

            errorMessage = "";
            return true;
        }

        public override bool SetupServer(out string errorMessage)
        {
            ServerInitializer.Initialize(new HexagramEntranceServerInitializer());
            return ServerInitializer.Instance.Initialize(out errorMessage);
        }

        public override void TearDown()
        {
            EndPointFactory.Instance.OnSubjectAdded -= LogEndPointConnected;
            EndPointFactory.Instance.OnSubjectRemoved -= LogEndPointDisconnected;
        }

        private void LogEndPointDisconnected(TerminalEndPoint endPoint)
        {
            HexagramEntranceServerApplication.Log.Info($"EndPoint: {endPoint} disconnected");
        }

        private void LogEndPointConnected(TerminalEndPoint endPoint)
        {
            HexagramEntranceServerApplication.Log.Info($"EndPoint: {endPoint} connected");
        }

        public override bool SetupDatabase(out string errorMessage)
        {
            ThroneDataConnection<MySqlConnection>.Initialize(new MariaDbThroneDataConnection());

            AnswerRepository.Initialize(new MariaDbAnswerRepository());

            return ThroneDataConnection<MySqlConnection>.Instance.Connect(
                serverAddress: ServerEnvironmentConfiguration.Instance.DatabaseServerAddress,
                port: ServerEnvironmentConfiguration.Instance.DatabasePort,
                username: ServerEnvironmentConfiguration.Instance.DatabaseUsername,
                password: ServerEnvironmentConfiguration.Instance.DatabasePassword,
                databasePrefix: ServerEnvironmentConfiguration.Instance.DatabasePrefix,
                charset: ServerEnvironmentConfiguration.Instance.DatabaseCharset,
                errorMessage: out errorMessage);
        }
    }
}

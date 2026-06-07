using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace SharpMap
{
	public static class ApiModHelper
	{
		static ICoreServerAPI? _sApi;
		public static ICoreServerAPI SApi
		{
			get => _sApi ?? throw new Exception("SApi is null");
			set => _sApi = value;
		}

		static ICoreClientAPI? _cApi;
		public static ICoreClientAPI CApi
		{
			get => _cApi ?? throw new Exception("CApi is null");
			set => _cApi = value;
		}

		static Mod? _mod;
		public static Mod Mod
		{
			get => _mod ?? throw new Exception("Mod is null");
			set => _mod = value;
		}

		public static void Error(string message) => Mod.Logger.Error(message);
		public static void Error(Exception ex) => Mod.Logger.Error(ex);
		public static void Warning(string message) => Mod.Logger.Warning(message);
		public static void Notification(string message) => Mod.Logger.Notification(message);

		public static string ModId => Mod.Info.ModID;

		public static IClientNetworkChannel GetClientChannel() => CApi.Network.GetChannel(ModId);
		public static IServerNetworkChannel GetServerChannel() => SApi.Network.GetChannel(ModId);


	}
}
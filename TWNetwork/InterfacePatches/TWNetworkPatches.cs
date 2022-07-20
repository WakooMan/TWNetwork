﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TWNetwork.Messages.FromClient;
using TWNetwork.Messages.FromServer;
using TWNetworkPatcher;
using static TaleWorlds.MountAndBlade.Agent;

namespace TWNetwork.InterfacePatches
{
    internal class TWNetworkPatches: HarmonyPatches
    {
		private bool GotTickMessage = false;


		[PatchedMethod(typeof(MissionState), "OnTick", true)]
		private void OnTick(float dt)
		{
			Run = GameNetwork.IsServer || GotTickMessage;
		}


		[PatchedMethod(typeof(MissionNetworkComponent), nameof(MissionNetworkComponent.OnMissionTick), false)]
		private void OnMissionTick(float dt)
		{
			if (GameNetwork.IsClient && GameNetwork.MyPeer.ControlledAgent != null)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new LookDirectionChangeRequest(Agent.Main));
				GameNetwork.EndModuleEventAsClient();
			}
			if (GameNetwork.IsServer)
			{
				GameNetwork.BeginBroadcastModuleEvent();
				GameNetwork.WriteMessage(new ServerTick(Mission.Current));
				GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
			}
		}

		[PatchedMethod(typeof(MissionNetworkComponent), "AddRemoveMessageHandlers", false)]
		private void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
		{
			if (GameNetwork.IsClient)
			{
				registerer.Register(new GameNetworkMessage.ServerMessageHandlerDelegate<ServerTick>(HandleServerEventServerTick));
			}
			if (GameNetwork.IsServer)
			{
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<MovementFlagChangeRequest>(HandleClientEventChangeMovementFlagChangeRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<EventFlagChangeRequest>(HandleClientEventEventFlagChangeRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<LookDirectionChangeRequest>(HandleClientEventLookDirectionChangeRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<WieldNextWeaponRequest>(HandleClientEventWieldNextWeaponRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<TryToWieldWeaponInSlotRequest>(HandleClientEventTryToWieldWeaponInSlotRequest));
				registerer.Register(new GameNetworkMessage.ClientMessageHandlerDelegate<TryToSheathWeaponInSlotRequest>(HandleClientEventTryToSheathWeaponInHandRequest));
			}
		}

		private static void HandleServerEventServerTick(ServerTick serverTick)
		{
			foreach (ServerAgentTick tick in serverTick.ServerAgentTicks)
			{
				tick.Agent.TeleportToPosition(tick.Position);
				tick.Agent.MovementFlags = tick.MovementFlags;
				tick.Agent.EventControlFlags = tick.EventControlFlags;
				tick.Agent.MovementInputVector = tick.MovementInputVector;
				tick.Agent.LookDirection = tick.LookDirection;
			}
			
		}

		private static bool HandleClientEventChangeMovementFlagChangeRequest(NetworkCommunicator networkPeer, MovementFlagChangeRequest changeMovementFlag)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.MovementFlags = changeMovementFlag.MovementFlag;
			}
			return true;
		}

		private static bool HandleClientEventEventFlagChangeRequest(NetworkCommunicator networkPeer, EventFlagChangeRequest changeEventFlag)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.EventControlFlags = changeEventFlag.EventFlag;
			}
			return true;
		}

		private static bool HandleClientEventWieldNextWeaponRequest(NetworkCommunicator networkPeer, WieldNextWeaponRequest wieldNextWeaponRequest)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.WieldNextWeapon(wieldNextWeaponRequest.WeaponIndex,wieldNextWeaponRequest.WieldActionType);
			}
			return true;
		}

		private static bool HandleClientEventTryToWieldWeaponInSlotRequest(NetworkCommunicator networkPeer, TryToWieldWeaponInSlotRequest tryToWieldWeaponInSlotRequest)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.TryToWieldWeaponInSlot(tryToWieldWeaponInSlotRequest.SlotIndex, tryToWieldWeaponInSlotRequest.Type,tryToWieldWeaponInSlotRequest.IsWieldedOnSpawn);
			}
			return true;
		}

		private static bool HandleClientEventTryToSheathWeaponInHandRequest(NetworkCommunicator networkPeer, TryToSheathWeaponInSlotRequest tryToSheathWeaponInSlotRequest)
		{
			if(networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.TryToSheathWeaponInHand(tryToSheathWeaponInSlotRequest.HandIndex, tryToSheathWeaponInSlotRequest.Type);
			}
			return true;
		}


		private static bool HandleClientEventLookDirectionChangeRequest(NetworkCommunicator networkPeer, LookDirectionChangeRequest lookDirectionChangeRequest)
		{
			if (networkPeer.ControlledAgent != null)
			{
				networkPeer.ControlledAgent.LookDirection = lookDirectionChangeRequest.LookDirection;
			}
			return true;
		}
	}
}
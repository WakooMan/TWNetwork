# TWNetwork
## Summary
Class Library to use the built in GameNetwork in Mount and Blade 2: Bannerlord on any kind of Server and Client System.
This repository is going to use [Protobuf-net](https://github.com/protobuf-net/protobuf-net) library for serializing messages and [HarmonyLib](https://harmony.pardeike.net/articles/intro.html) library to patch out
the methods for GameNetwork class and other classes about networking that is related to missions in Mount and Blade 2: Bannerlord.
## Problem
I want to be able to create a server on my local computer that can use the GameNetwork class without the NetworkMain class.
Basically, I want to separate my server from TaleWorld's server. (NetworkMain class is connecting to the lobby server of TaleWorlds)
## Already Done
- I made the GameNetworkMessage classes serializable by Protobuf-net on runtime.
- For that I wrote plenty of surrogate types (A type that is going to be serialized, when the main type object gets serialized) for TaleWorld classes.
- Searched for methods in GameNetwork that needs to be patched out, these methods are:
  - BeginBroadcastModuleEvent
  - BeginModuleEventAsClient
  - BeginModuleEventAsClientUnreliable
  - EndBroadcastModuleEvent
  - EndBroadcastModuleEventUnreliable
  - EndModuleEventAsClient
  - EndModuleEventAsClientUnreliable
  - IncreaseTotalUploadLimit
  - InitializeClientSide
  - InitializeServerSide
  - PrepareNewUdpSession
  - PrintDebugStats
  - PrintReplicationTableStatistics
  - ResetDebugUploads
  - ResetDebugVariables
  - ClearReplicationTableStatistics
  - AddPeerToDisconnect
  - AddNewPlayerOnServer
  - AddNewPlayersOnServer
  - TerminateServerSide
  - TerminateClientSide
  - GetAveragePacketLossRatio
  - GetDebugUploadsInBits
  - ResetMissionData
  - HandleNetworkPacketAsServer
  - IsServer
  - IsClient
- Found the best way to patch out multiple methods in one class [here at Patching multiple methods](https://harmony.pardeike.net/articles/annotations.html). Probably [Finalizer](https://harmony.pardeike.net/articles/patching-finalizer.html) and [Reverse Patch](https://harmony.pardeike.net/articles/reverse-patching.html) can be useful in the future as well.
## In Progress
- Find ways to handle multiple Missions on server side.
- Making class Diagram
## Plan
- Collect the methods that needs to be patched out with dnSpy. (done)
- Find the best way to patch out these methods with HarmonyLib. (done)
- Find ways to handle multiple Missions on server side.(Inheritence,Extensions or patches? Find the best structure to run missions in the background)
- Create Initial Class Diagram.
- Code and debug.
- Test in missions.
- Try to make this repository extensible. (Mainly the serializers are the ones that is problematic)

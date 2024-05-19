# GServer

**Note:** still writing README...

A rudimentary UDP game server mimicking the architecture of Graal Online's GServer from the 90s-00s.

## Networking

UDP packets are sent between the client and server to enable communication. UDP requests can be though of like 'messages', where each message has an ID that denotes the type of message (e.g. AUTH, LIST_SERVERS), followed by the message's data.

The first byte of a message is used to denote its type, but no standard format is shared between messages for the data belonging to each message.

For example: when authenticating, the following format is used:

- Message ID (1 byte)
- Username length (1 byte)
- Username (x bytes, according to the username length)
- Password length (1 byte)
- Password (x bytes, according to the password length)

`{message_id}{username_len}{username}{password_len}{password}`

All possible client and server message IDs can be found in `Networking/Enums/ClientPacketIn.cs` or `Networking/Enums/ServerPacketIn.cs` respectively.

## Projects

The repo is made up of the following projects:
* GServer.Server - the server to be connected to
* GServer.Client - the client that connects to the server
* GServer.Common - a class library used by both of the above projects. Contains shared code.

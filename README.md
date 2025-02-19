# GServer

**Note:** This README is a work in progress 🛠️

A rudimentary TCP/UDP game server mimicking the architecture of Graal Online's GServer from the 90s-00s.

## Networking

Packets are sent between the client and server to enable communication. Specific packets can carry data specific purposes, referred to as messages. A message has an ID that defines its type/purpose, as well as data related to its purpose.

The first byte of a message is used to denote its type, but no standard format is shared between messages for the data belonging to each message.

For example: when authenticating, the following message is used:

- Message ID (1 byte)
- Username length (1 byte)
- Username (x bytes, according to the username length)
- Password length (1 byte)
- Password (x bytes, according to the password length)

`{message_id}{username_len}{username}{password_len}{password}`

(see *Messages* section for this message and others' ASN.1 definitions)

All possible client and server message IDs can be found in `Networking/Enums/ClientPacketIn.cs` or `Networking/Enums/ServerPacketIn.cs`, according to whether the message is being received (_in_) or transmitted (_out_)

### Messages

*AuthMessage*

```asn.1
AuthMessage ::= SEQUENCE {
    messageId   INTEGER (0..255),
    usernameLen INTEGER (0..255),
    username    OCTET STRING (SIZE (0..usernameLen)),
    passwordLen INTEGER (0..255),
    password    OCTET STRING (SIZE (0..passwordLen))
}
```

*ListServerMessage* (TODO)

```asn.1
ServerListingBlock ::= SEQUENCE {
    name        OCTET STRING (SIZE (50)),
    description OCTET STRING (SIZE (1000)),
    playercount INTEGER (0..65535),
    ip          OCTET STRING (SIZE (15)),
    port        INTEGER (0..65535),
    serverTier  INTEGER (0..255)
}

ServerListing ::= SEQUENCE {
    messageId   INTEGER (0..255),
    ...
}
```

## Projects

The repo is made up of the following projects:
* GServer.Server - the server to be connected to
* GServer.Client - the client that connects to the server
* GServer.Common - a class library used by both of the above projects. Contains shared code.

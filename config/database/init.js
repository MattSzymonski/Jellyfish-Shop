﻿// Creates new entry in admin/system.users
db.createUser(
    {
        user: "JellyfishShopAdmin",
        pwd: "123",
        roles: [
            {
                role: "readWrite",
                db: "main"
            }
        ]
    }
);

// --- Create database, collections, and documents ---

// - Main database -
db = db.getSiblingDB('jellyfishShopMain'); 

db.createCollection("users");
db.users.insert([
   { _id: ObjectId("606b498d982ca2c8da77e631"), Login: "c.po@pope.va", Password: "7312", FirstName: "Cap'n Carl", LastName: "Wojtylarr", RegistrationDate: ISODate("2005-04-02T21:37:00.000Z") },
   { _id: ObjectId("606b498d982ca2c8da77e632"), Login: "reginald.bartholomew@mail.va", Password: "1234", FirstName: "Reginald", LastName: "Bartholomew", RegistrationDate: ISODate("2005-04-02T21:37:00.000Z") },
   { _id: ObjectId("606b498d982ca2c8da77e633"), Login: "salt.mcgregor@mail.com", Password: "salty", FirstName: "Salt", LastName: "McGregor", RegistrationDate: ISODate("2023-05-06T17:48:19.872Z") },
   { _id: ObjectId("606b498d982ca2c8da77e634"), Login: "t.se@mail.com", Password: "SailingTrains",  FirstName: "Thomas", LastName: "Steam Engine", RegistrationDate: ISODate("2023-05-06T17:48:19.872Z") },
]);

db.createCollection("refreshTokens");
db.refreshTokens.createIndex( { "tokenString": 1 }, { unique: true } )

db.createCollection("jellyfish");
db.jellyfish.insert([
    { _id: ObjectId("63f0f932c00f797161eaa726"), Name: "Moon Glow", Behaviour: "Aggressive", SellingUserId: ObjectId("606b498d982ca2c8da77e633"), AddDate: ISODate("2023-02-18T16:13:00.000Z"), Price: 13.20, ShopNote: "Great quality" },
    { _id: ObjectId("63f0f932c00f797161eaa727"), Name: "Moon Glow", Behaviour: "Aggressive", SellingUserId: ObjectId("606b498d982ca2c8da77e631"), AddDate: ISODate("2023-01-20T13:16:00.000Z"), Price: 26.00, ShopNote: "Poor quality, but very large" },
    { _id: ObjectId("63f0f932c00f797161eaa728"), Name: "Pink Puff", Behaviour: "Neutral", SellingUserId: ObjectId("606b498d982ca2c8da77e631"), AddDate: ISODate("2023-01-21T11:22:00.000Z"), Price: 20.00, ShopNote: "" },
    { _id: ObjectId("63f0f932c00f797161eaa729"), Name: "Pink Puff", Behaviour: "Fleeing", SellingUserId: ObjectId("606b498d982ca2c8da77e633"), AddDate: ISODate("2023-01-19T23:45:00.000Z"), Price: 5.00, ShopNote: "Tiny, no one will buy it :/" },
    { _id: ObjectId("63f0f932c00f797161eaa721"), Name: "Pink Puff", Behaviour: "Aggressive", SellingUserId: ObjectId("606b498d982ca2c8da77e632"), AddDate: ISODate("2023-03-27T04:31:00.000Z"), Price: 8.50, ShopNote: "" },
    { _id: ObjectId("63f0f932c00f797161eaa722"), Name: "Moon Glow", Behaviour: "Fleeing", SellingUserId: ObjectId("606b498d982ca2c8da77e633"), AddDate: ISODate("2023-02-12T09:23:00.000Z"), Price: 50.50, ShopNote: "Bought from suspicious fisherman" },
]);
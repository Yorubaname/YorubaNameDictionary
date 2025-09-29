// Current date
const currentDate = ISODate();
db = db.getSiblingDB("yoruba_name_dictionary");
yorubawordDb = db.getSiblingDB("yoruba_dictionary");

// Users collection initialization
const users = [
    {
        "_id": "669a1c2ebfcf74446326d088",
        "CreatedAt": currentDate,
        "CreatedBy": "Hafiz is initializing",
        "UpdatedAt": currentDate,
        "UpdatedBy": null,
        "Email": "admin@yorubaname.com",
        "Username": "admin",
        "Password": "$2a$10$SnjXPE8WzS/5cg9BQdS/jO.Wc8ohiyhqy62mlG7rTEvX0jGWS3KqW",
        "Roles": ["ADMIN"]
    },
    {
        "_id": "669a1c2ebfcf74446326d089",
        "CreatedAt": currentDate,
        "CreatedBy": "Hafiz is initializing",
        "UpdatedAt": currentDate,
        "UpdatedBy": null,
        "Email": "basicuser@yorubaname.com",
        "Username": "basicuser",
        "Password": "$2a$10$SnjXPE8WzS/5cg9BQdS/jO.Wc8ohiyhqy62mlG7rTEvX0jGWS3KqW",
        "Roles": ["BASIC_LEXICOGRAPHER"]
    },
    {
        "_id": "669a1c2ebfcf74446326d090",
        "CreatedAt": currentDate,
        "CreatedBy": "Hafiz is initializing",
        "UpdatedAt": currentDate,
        "UpdatedBy": null,
        "Email": "prolexuser@yorubaname.com",
        "Username": "prolexuser",
        "Password": "$2a$10$SnjXPE8WzS/5cg9BQdS/jO.Wc8ohiyhqy62mlG7rTEvX0jGWS3KqW",
        "Roles": ["PRO_LEXICOGRAPHER"]
    },
    {
        "_id": "669a1c2ebfcf74446326d091",
        "CreatedAt": currentDate,
        "CreatedBy": "Hafiz is initializing",
        "UpdatedAt": currentDate,
        "UpdatedBy": null,
        "Email": "deputyadmin@yorubaname.com",
        "Username": "deputyadmin",
        "Password": "$2a$10$SnjXPE8WzS/5cg9BQdS/jO.Wc8ohiyhqy62mlG7rTEvX0jGWS3KqW",
        "Roles": ["ADMIN"]
    },
    {
        "_id": "669a1c2ebfcf74446326d092",
        "CreatedAt": currentDate,
        "CreatedBy": "Hafiz is initializing",
        "UpdatedAt": currentDate,
        "UpdatedBy": null,
        "Email": "basiclexuser@yorubaname.com",
        "Username": "basiclexuser",
        "Password": "$2a$10$SnjXPE8WzS/5cg9BQdS/jO.Wc8ohiyhqy62mlG7rTEvX0jGWS3KqW",
        "Roles": ["BASIC_LEXICOGRAPHER"]
    }
];
db.Users.insertMany(users);
yorubawordDb.Users.insertMany(users);

// GeoLocations collection initialization
const geoLocations = [
    {
        "_id": "669a1c65b90a6db2be3651e4",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "ABEOKUTA",
        "Region": "NWY"
    },
    {
        "_id": "669a1c65b90a6db2be3651e5",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "AKURE",
        "Region": "CY"
    },
    {
        "_id": "669a1c65b90a6db2be3651e6",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "EFON",
        "Region": "CY"
    },
    {
        "_id": "669a1c65b90a6db2be3651e7",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "EKITI",
        "Region": "CY"
    },
    {
        "_id": "669a1c65b90a6db2be3651e8",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "EKO",
        "Region": "EKO"
    },
    {
        "_id": "669a1c65b90a6db2be3651e9",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "FOREIGN-ARABIC",
        "Region": "FOREIGN_ARABIC"
    },
    {
        "_id": "669a1c65b90a6db2be3651ea",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "FOREIGN-GENERAL",
        "Region": "FOREIGN_GENERAL"
    },
    {
        "_id": "669a1c65b90a6db2be3651eb",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "GENERAL",
        "Region": "GENERAL"
    },
    {
        "_id": "669a1c65b90a6db2be3651ec",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "I DO NOT KNOW",
        "Region": "UNDEFINED"
    },
    {
        "_id": "669a1c65b90a6db2be3651ed",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "IBADAN",
        "Region": "NWY"
    },
    {
        "_id": "669a1c65b90a6db2be3651ee",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "IFE",
        "Region": "CY"
    },
    {
        "_id": "669a1c65b90a6db2be3651ef",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "IGBOMINA",
        "Region": "CY"
    },
    {
        "_id": "669a1c65b90a6db2be3651f0",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "IJALE",
        "Region": "SEY"
    },
    {
        "_id": "669a1c65b90a6db2be3651f1",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "IJEBU",
        "Region": "CY"
    },
    {
        "_id": "669a1c65b90a6db2be3651f2",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "IKARE",
        "Region": "SEY"
    },
    {
        "_id": "669a1c65b90a6db2be3651f3",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "ILESHA",
        "Region": "CY"
    },
    {
        "_id": "669a1c65b90a6db2be3651f4",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "OGUN",
        "Region": "OGN"
    },
    {
        "_id": "669a1c65b90a6db2be3651f5",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "OKITIPUPA",
        "Region": "SEY"
    },
    {
        "_id": "669a1c65b90a6db2be3651f6",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "ONDO",
        "Region": "SEY"
    },
    {
        "_id": "669a1c65b90a6db2be3651f7",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "OTHERS",
        "Region": "OTHERS"
    },
    {
        "_id": "669a1c65b90a6db2be3651f8",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "OWO",
        "Region": "SEY"
    },
    {
        "_id": "669a1c65b90a6db2be3651f9",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "OYO",
        "Region": "OYO"
    },
    {
        "_id": "669a1c65b90a6db2be3651fa",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "SAGAMU",
        "Region": "SEY"
    },
    {
        "_id": "669a1c65b90a6db2be3651fb",
        "CreatedBy": "SQLTOMongoMigrator",
        "Place": "YAGBA",
        "Region": "CY"
    }
];
db.GeoLocations.insertMany(geoLocations);

const nameEntries = [
    {
        "_id": "669050feeb1b60417574458f",
        "Title": "Ààrẹ",
        "Pronunciation": "",
        "IpaNotation": "",
        "Meaning": "Commander.",
        "ExtendedMeaning": "Aàrẹ is a title, usually in the military of the Yoruba warriors company. The leader of the company from a clan takes the title when they join up with a coalition.",
        "Morphology": [
            "ààrẹ"
        ],
        "MediaLinks": [
            {
                "Url": "https://en.wikipedia.org/wiki/Bola_Are"
            }
        ],
        "State": 2,
        "Etymology": [
            {
                "Part": "aàrẹ",
                "Meaning": "commander"
            }
        ],
        "GeoLocation": [
            {
                "Place": "OTHERS",
                "Region": "OTHERS"
            }
        ],
        "FamousPeople": [
            "Bọ́lá Àrẹ",
            "gospel musician"
        ],
        "Syllables": [
            "a",
            "à",
            "rẹ"
        ],
        "VariantsV2": [
            {
                "Title": "Àrẹ"
            }
        ],
        "Feedbacks": []
    },
    {
        "_id": "669050feeb1b604175744590",
        "Title": "Àbáyọ̀mí",
        "Pronunciation": "abayomi",
        "IpaNotation": "",
        "Meaning": "I would have been mocked.",
        "ExtendedMeaning": "It is a name given to a child born after a number of unfortunate or near unfortunate circumstances. It is often called in full as Àbáyòmí Olúwaniòjé: I would have been mocked, if not for God.",
        "Morphology": [
            "à-bá-yọ̀-mí"
        ],
        "MediaLinks": [
            {
                "Url": "https://en.wikipedia.org/wiki/Kofo_Abayomi"
            },
            {
                "Url": "http://www.vanguardngr.com/2016/08/tunji-abayomi-tinubu-never-sought-support-allow-people-freely-fairly-constitute-government/"
            }
        ],
        "State": 2,
        "Etymology": [
            {
                "Part": "a",
                "Meaning": "we/they"
            },
            {
                "Part": "bá",
                "Meaning": "would have"
            },
            {
                "Part": "yọ̀",
                "Meaning": "mock"
            },
            {
                "Part": "mí",
                "Meaning": "me"
            }
        ],
        "GeoLocation": [
            {
                "Place": "OTHERS",
                "Region": "OTHERS"
            }
        ],
        "FamousPeople": [
            "Kòfó Àbáyọ̀mí(Politician)",
            "Túnjí Àbáyọ̀mí(Politician)"
        ],
        "Syllables": [
            "à",
            "bá",
            "yọ̀",
            "mí"
        ],
        "VariantsV2": [
            {
                "Title": "Yọ̀mí"
            },
            {
                "Title": "Báyọ̀"
            }
        ],
        "Feedbacks": [
            {
                "_id": "3134",
                "CreatedAt": ISODate("2024-07-11T21:38:59.326Z"),
                "CreatedBy": null,
                "UpdatedAt": ISODate("2024-07-11T21:38:59.326Z"),
                "UpdatedBy": null,
                "Content": "Adhurojindakimero"
            },
            {
                "_id": "3135",
                "CreatedAt": ISODate("2024-07-11T21:38:59.326Z"),
                "CreatedBy": null,
                "UpdatedAt": ISODate("2024-07-11T21:38:59.326Z"),
                "UpdatedBy": null,
                "Content": "Adhuro jindaki mero"
            }
        ]
    },
    {
        "_id": "669050feeb1b604175744591",
        "Title": "Abégúndé",
        "Pronunciation": "",
        "IpaNotation": null,
        "Meaning": "The one who came with the masquerade.",
        "ExtendedMeaning": "",
        "Morphology": [
            "a-bá-eégún-dé"
        ],
        "MediaLinks": [
            {
                "Url": "http://www.shineyoureye.org/person/ifedayo-sunday-abegunde/"
            },
            {
                "Url": "https://www2.deloitte.com/ng/en/profiles/femi-aabegunde.html"
            }
        ],
        "State": 2,
        "Etymology": [
            {
                "Part": "a",
                "Meaning": "one who"
            },
            {
                "Part": "bá",
                "Meaning": "together with"
            },
            {
                "Part": "eégún",
                "Meaning": "masquerade"
            },
            {
                "Part": "dé",
                "Meaning": "come, arrive"
            }
        ],
        "GeoLocation": [
            {
                "Place": "OTHERS",
                "Region": "OTHERS"
            }
        ],
        "FamousPeople": [
            "Ìfẹ́dayọ̀ Abégúdé",
            "Olúfẹ́mi Abégúdé"
        ],
        "Syllables": [
            "a",
            "bé",
            "gún",
            "dé"
        ],
        "VariantsV2": [],
        "Feedbacks": []
    },
    {
        "_id": "669050feeb1b604175744592",
        "Title": "Abẹ̀rùàgbà",
        "Pronunciation": "",
        "IpaNotation": "",
        "Meaning": "The one who respects elders.",
        "ExtendedMeaning": "A nickname. Abẹ̀rùàgbà is the opening phrase in the proverbial sentence: Abẹ̀rùàgbà ni yó tẹlẹ̀ yí pẹ́ (only the respectful will live long).",
        "Morphology": [
            "a-bẹ̀rù-àgbà"
        ],
        "MediaLinks": [
            {
                "Url": "https://www.facebook.com/rotimi.aberuagba"
            }
        ],
        "State": 2,
        "Etymology": [
            {
                "Part": "a",
                "Meaning": "one who"
            },
            {
                "Part": "bẹrù",
                "Meaning": "fear, be afraid"
            },
            {
                "Part": "àgbà",
                "Meaning": "elder"
            }
        ],
        "GeoLocation": [
            {
                "Place": "OTHERS",
                "Region": "OTHERS"
            }
        ],
        "FamousPeople": [
            "Rótìmí Abẹ̀rùàgbà(Media)"
        ],
        "Syllables": [
            "a",
            "bẹ̀",
            "rù",
            "à",
            "gbà"
        ],
        "VariantsV2": [],
        "Feedbacks": []
    },
    {
        "_id": "669050feeb1b604175744593",
        "Title": "Abíádé",
        "Pronunciation": "",
        "IpaNotation": "",
        "Meaning": "One born to royalty.",
        "ExtendedMeaning": "It's a name associated with royal children.",
        "Morphology": [
            "a-bí-(sí)-adé"
        ],
        "MediaLinks": [
            {
                "Url": "http://independentnig.com/i-am-proud-i-walked-out-of-my-abusive-marriage-abiola-abiade/"
            },
            {
                "Url": "https://mie.uic.edu/k-teacher/jeremiah-abiade-phd/"
            }
        ],
        "State": 2,
        "Etymology": [
            {
                "Part": "a",
                "Meaning": "we"
            },
            {
                "Part": "bí",
                "Meaning": "gave birth to (this)"
            },
            {
                "Part": "sí",
                "Meaning": "into"
            },
            {
                "Part": "adé",
                "Meaning": "crown, royalty"
            }
        ],
        "GeoLocation": [
            {
                "Place": "OTHERS",
                "Region": "OTHERS"
            }
        ],
        "FamousPeople": [
            "Abíọ́lá Abíádé(Social Agent)", "Jeremiah Abíádé(Professor)"
        ],
        "Syllables": [
            "a",
            "bí",
            "á",
            "dé"
        ],
        "VariantsV2": [
            {
                "Title": "Abísádé"
            },
            {
                "Title": "Bíádé"
            },
            {
                "Title": "Bisade"
            }
        ],
        "Feedbacks": []
    },
    {
        "_id": "669050feeb1b604175744594",
        "Title": "Àbíbátù",
        "Pronunciation": null,
        "IpaNotation": null,
        "Meaning": "The Yorùbá version of Habeebatu, an Arabic name meaning \"love\".",
        "ExtendedMeaning": "the name actually means friend. the complete name is Habeeb-llahi meaning \"friend of God\". In normal daily Arabic conversation, if you want to say my friend,  you'd say \"Yah habibi\"",
        "Morphology": [
            "[unknown]"
        ],
        "MediaLinks": [
            {
                "Url": "https://en.wikipedia.org/wiki/Abibatu_Mogaji"
            }
        ],
        "State": 0,
        "Etymology": [
            {
                "Part": "",
                "Meaning": ""
            }
        ],
        "GeoLocation": [
            {
                "Place": "OTHERS",
                "Region": "OTHERS"
            }
        ],
        "FamousPeople": [
            "Àbíbátù Mọ́gàjí"
        ],
        "Syllables": [
            "à",
            "bí",
            "bá",
            "tù"
        ],
        "VariantsV2": [],
        "Feedbacks": []
    },
    {
        "_id": "669050feeb1b604175744595",
        "Title": "Abídèmí",
        "Pronunciation": "",
        "IpaNotation": "",
        "Meaning": "The child born before I returned.",
        "ExtendedMeaning": "It's a name typically given to a child born when the father was away out of town/country.",
        "Morphology": [
            "a-bí-dè-mí"
        ],
        "MediaLinks": [
            {
                "Url": "https://en.wikipedia.org/wiki/Abidemi_Sanusi"
            },
            {
                "Url": "https://www.linkedin.com/in/oderinlo"
            }
        ],
        "State": 2,
        "Etymology": [
            {
                "Part": "a",
                "Meaning": "we"
            },
            {
                "Part": "bí",
                "Meaning": "gave birth to (this)"
            },
            {
                "Part": "dè",
                "Meaning": "waiting for"
            },
            {
                "Part": "mí",
                "Meaning": "me"
            }
        ],
        "GeoLocation": [
            {
                "Place": "IBADAN",
                "Region": "NWY"
            }
        ],
        "FamousPeople": [
            "Abídèmí Sànúsí (Author)",
            "Abídèmí Ọdẹ́rìndé(IT Consultant)"
        ],
        "Syllables": [
            "a",
            "bí",
            "dè",
            "mí"
        ],
        "VariantsV2": [
            {
                "Title": "Bídèmí"
            }
        ],
        "Feedbacks": []
    },
    {
        "_id": "669050feeb1b604175744596",
        "Title": "Abíkóyè",
        "Pronunciation": "",
        "IpaNotation": "",
        "Meaning": "Given birth to in addition to the chieftaincy.",
        "ExtendedMeaning": "It's most likely given to a child born after the parent has earned a notable chieftaincy role (or kingship)",
        "Morphology": [
            "a-bí-kún-oyè"
        ],
        "MediaLinks": [],
        "State": 2,
        "Etymology": [
            {
                "Part": "a",
                "Meaning": "one who"
            },
            {
                "Part": "bí",
                "Meaning": "give birth to"
            },
            {
                "Part": "kún",
                "Meaning": "in addition to"
            },
            {
                "Part": "oyè",
                "Meaning": "honour, respect, chieftaincy"
            }
        ],
        "GeoLocation": [
            {
                "Place": "OTHERS",
                "Region": "OTHERS"
            }
        ],
        "FamousPeople": [
            "Abíkóyè Ọlámǐdé(Chartered Accountant)"
        ],
        "Syllables": [
            "a",
            "bí",
            "kó",
            "yè"
        ],
        "VariantsV2": [],
        "Feedbacks": []
    }
];

db.NameEntries.insertMany(nameEntries);
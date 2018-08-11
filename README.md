TaterSlackBots
==============

Just some Slack Bots to help you out and brighten your day!

StanLeeSlackBot
---------------
In SB.StanLee you'll need to create an appsettings.json file (along with the variety of environments for Development and Production). The template is:

```json
{
  "Serilog": {
    "MinimumLevel": {
        "Default": "Information",
        "Override": {
            "Microsoft": "Warning",
            "System": "Error"
      }
    }
  },
  "AppSettingType": "Production",
  "ServiceConfig": {
      "StanLeeConfig": {
          "Name": "StanLeeSlackBot",
          "DisplayName": "Stan Lee SlackBot",
          "Description": "An unofficial Stan Lee Slackbot"
      }
  },
  "Marvel": {
    "PublicKey": "",
    "PrivateKey": ""
  },
  "Slack": {
    "ApiToken": ""
  },
  "ApplicationInsights": {
    "InstrumentationKey": ""
  }
}
```

### To Install
From an elevated command prompt:
sc create StanLeeBot binPath= "{path}\StanLeeBot\StanLee.exe"

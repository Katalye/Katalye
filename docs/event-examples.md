salt/key
```json
{
  "result": true,
  "_stamp": "2018-07-08T20:24:54.748591",
  "id": "k4",
  "act": "delete"
}.
```

key
```json
{
  "_stamp": "2018-07-08T20:26:01.124262",
  "rotate_aes_key": true
}
```

salt/job/20180708041923644480/ret/d1
```json
{
  "fun_args": [],
  "jid": "20180708041923644480",
  "return": true,
  "retcode": 0,
  "success": true,
  "cmd": "_return",
  "_stamp": "2018-07-08T04:19:28.61852",
  "fun": "test.ping",
  "id": "d1"
}
```

salt/job/20180708042042280933/new
```json
{
  "tgt_type": "glob",
  "jid": "20180708042042280933",
  "tgt": "d1",
  "missing": [],
  "_stamp": "2018-07-08T04:20:42.283007",
  "user": "root",
  "arg": [],
  "fun": "test.ping",
  "minions": [
    "d1"
  ]
}
```

20180708042042280933
```json
{
  "_stamp": "2018-07-08T04:20:42.282471",
  "minions": [
    "d1"
  ]
}
```

salt/auth
```json
{
  "result": true,
  "_stamp": "2018-07-08T04:26:40.06792",
  "id": "d1",
  "pub": "-----BEGIN PUBLIC KEY-----\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmDO5zOYAdNkk2PlzTg1i\nnvmpCt1aZNdZjA+F0lDeK3fURyg3mMqVQiDGc+Du5qlg+R/752bM77wmFoi3AlXL\nMlO5FB1Ap6kUvOTWxsAYOVBCtOsF4AeMYx0t5ny7X4uzN7lQeZND9Kko4E8mXiZ5\nD4ZSRKKVwXcyTEbm3fZQWhHmz5th1nHYMuRTknfWbvrxG5BJGrYSBNdShIJDm+Pf\n2wSD01OeCCPBXZ/ZMDQXCQFv3vmNip458X3HuhxAZ+v3IKhJk2hPTWTlSrAjV29Y\nnBoNUbF1X2UpX6sdFz8yJeAN1g2fMAUGNjyprBf9IS7mIbA0RO5IUhrtJmB2rYzL\ncwIDAQAB\n-----END PUBLIC KEY-----",
  "act": "accept"
}
```

minion/refresh/d1, {
  "Minion data cache refresh": "d1",
  "_stamp": "2018-07-08T20:40:27.249673"
}

salt/job/20180708204022106325/prog/d1/0, {
  "jid": "20180708204022106325",
  "cmd": "_minion_event",
  "_stamp": "2018-07-08T20:40:28.555277",
  "tag": "salt/job/20180708204022106325/prog/d1/0",
  "data": {
    "ret": {
      "comment": "AllowTelemetry in HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection is already configured",
      "name": "HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection",
      "start_time": "15:40:27.496000",
      "result": true,
      "duration": 0.0,
      "__run_num__": 0,
      "__sls__": "infra.common",
      "changes": {},
      "__id__": "Ensure enhanced telemetry is disabled"
    },
    "len": 2
  },
  "id": "d1"
}

salt/job/20180708204022106325/prog/d1/1, {
  "jid": "20180708204022106325",
  "cmd": "_minion_event",
  "_stamp": "2018-07-08T20:40:29.506823",
  "tag": "salt/job/20180708204022106325/prog/d1/1",
  "data": {
    "ret": {
      "comment": "RDP is enabled",
      "name": "Ensure Remote management via rdp is enabled",
      "start_time": "15:40:28.637000",
      "result": true,
      "duration": 849.0,
      "__run_num__": 1,
      "__sls__": "infra.common",
      "changes": {},
      "__id__": "Ensure Remote management via rdp is enabled"
    },
    "len": 2
  },
  "id": "d1"
}

20180708204022106325, d1, {
  "fun_args": [
    "infra.common"
  ],
  "jid": "20180708204022106325",
  "return": {
    "reg_|-Ensure enhanced telemetry is disabled_|-HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection_|-present": {
      "comment": "AllowTelemetry in HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection is already configured",
      "name": "HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection",
      "start_time": "15:40:27.496000",
      "result": true,
      "duration": 0.0,
      "__run_num__": 0,
      "__sls__": "infra.common",
      "changes": {},
      "__id__": "Ensure enhanced telemetry is disabled"
    },
    "rdp_|-Ensure Remote management via rdp is enabled_|-Ensure Remote management via rdp is enabled_|-enabled": {
      "comment": "RDP is enabled",
      "name": "Ensure Remote management via rdp is enabled",
      "start_time": "15:40:28.637000",
      "result": true,
      "duration": 849.0,
      "__run_num__": 1,
      "__sls__": "infra.common",
      "changes": {},
      "__id__": "Ensure Remote management via rdp is enabled"
    }
  },
  "retcode": 0,
  "success": true,
  "cmd": "_return",
  "_stamp": "2018-07-08T20:40:29.558991",
  "fun": "state.apply",
  "id": "d1",
  "out": "highstate"
}

{
  "fun_args": [],
  "jid": "20180708041923644480",
  "return": true,
  "retcode": 0,
  "success": true,
  "cmd": "_return",
  "_stamp": "2018-07-08T04:19:28.61852",
  "fun": "test.ping",
  "id": "d1"
}
---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by xieliujian.
--- DateTime: 2019/8/11 22:31
---
---
local skynet = require "skynet"

-- 启动服务(启动函数)
skynet.start(function()

    -- 启动函数里调用Skynet API开发各种服务
    print("======Server start=======")

    skynet.newservice("socket2")

    skynet.exit()
end)

---@type flatbuffers
local flatbuffers = require("flatbuffers");

---@type netmsg
local netmsg = require 'netmsg'

---@type socket
local socket = require "skynet.socket"

---@type netpack
local netpack = require "skynet.netpack"

---@class msgdispatcher
msgdispatcher = {

}

msgdispatcher.msgType = {
    flatbuffer = 0,
    protobuf = 1,
}

msgdispatcher.builder = flatbuffers.Builder(1024)

msgdispatcher.curMsgType = msgdispatcher.msgType.flatbuffer;

-- 消息分发
msgdispatcher.dispatcher = function(id, str)

    if msgdispatcher.curMsgType == msgdispatcher.msgType.flatbuffer then
        msgdispatcher.dispatcherFbMsg(id, str);
    end
end

-- fb消息注册
msgdispatcher.registerFbMsg = function(msg, handler)
    netmsg.AddListener(msg, handler);
end

-- fb消息取消注册
msgdispatcher.unRegisterFbMsg = function(msg, handler)
    netmsg.RemoveListener(msg, handler);
end

-- 发送fb消息
msgdispatcher.sendFbMsg = function(id, msg)

    local bufAsString = msgdispatcher.builder:Output();
    local msgid = msg.HashID;
    local strwrite = string.pack("<L", msgid);
    strwrite = strwrite .. bufAsString;

    local package = string.pack("<s2", strwrite)

    socket.write(id, package)
end

-- fb消息分发
msgdispatcher.dispatcherFbMsg = function(id, msg, sz)

    local str = netpack.tostring(msg, sz);

    -- string.unpack默认是1
    local msgid = string.unpack("<L", str);
    local msgoffset = 8;

    local msgbuf = flatbuffers.binaryArray.New(str);

    local eventlib = netmsg.getEvents(msgid);
    if not eventlib then
        return ;
    end

    local msgclass = eventlib.event;
    if not msgclass then
        return ;
    end

    local msg = msgclass.init(msgbuf, msgoffset);
    if not msg then
        return ;
    end

    local data = {};
    data.id = id;
    data.msg = msg;
    netmsg.Brocast(msgid, data);
end

return msgdispatcher;
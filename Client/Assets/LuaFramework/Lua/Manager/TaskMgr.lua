local TaskMgr = {}
local yieldCo = {}
local resumeCo = {}

local run = function(co)
    local result, stack = coroutine.resume(co)
    if not result then
        print(stack .. debug.traceback(co))
    end
end

function TaskMgr:update()
    resumeCo = {}
    for co, sec in pairs(yieldCo) do
        if sec > 0 then
            yieldCo[co] = sec - 1
        end
        if yieldCo[co] <= 0 then
            table.insert(resumeCo, co)
        end
    end
    for _, co in pairs(resumeCo) do
        yieldCo[co] = nil
        self:resume(co)
    end
end

function TaskMgr:runGameTask(func)
    self:resume(coroutine.create(function()
        func()
    end))
end

function TaskMgr:waitSeconds(sec)
    local co = coroutine.running()
    if co then
        yieldCo[co] = sec
        coroutine.yield()
    end
end

function TaskMgr:resume(co)
    pcall(run, co)
end

return TaskMgr
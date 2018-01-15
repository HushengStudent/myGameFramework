---
--- Created by husheng.
--- DateTime: 2018/1/8 23:49
---

require "Define"
require "Common/Class"

Game = class("Game")

function Game:ctor() end

game = Game.new()

function Game:StartGame()
    log("game start!")
end




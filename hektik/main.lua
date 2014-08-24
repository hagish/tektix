json = require('json')

Server = {}

function StartClient(player)
	Server.host, Server.port = "192.168.2.123", 25000 + player
	Server.socket = require("socket")
	Server.tcp = assert(Server.socket.tcp())
	Server.tcp:settimeout(0)
	Server.tcp:connect(Server.host, Server.port)
end

function SendMessage(msg)
	if Server.tcp then
		Server.tcp:send(msg)
	end
end

function UpdateClient()
	if Server.tcp then
		--while true do
			local msg, status, partial = Server.tcp:receive("*l")
			if msg then
				if onServerReceive then
					onServerReceive(json.decode(msg))
				end
			end
			--if status == "closed" then break end
		--end
	end
end

function QuitClient()
	if Server.tcp then
		Server.tcp:close()
	end
end

function CreatePhysicsRect(x, y, width, height)
	local object = {}

	object.body = love.physics.newBody(world, x, y, "static")
	object.shape = love.physics.newRectangleShape(0, 0, width, height)
	object.fixture = love.physics.newFixture(object.body, object.shape)

	return object
end

function CreatePhysicsCircle(x, y, radius)
	local object = {}

	object.body = love.physics.newBody(world, x, y, "static")
	object.shape = love.physics.newCircleShape(radius)
	object.fixture = love.physics.newFixture(object.body, object.shape)

	return object
end

-- balancing variables
playerMoveSpeed = 400
playerJumpPowerUp = 750
playerJumpPowerDown = 750
boxSpawnRateInSeconds = 0.75
playerWidth = 70
playerHeight = 72
boxMovementVelocity = 200
candyRadius = 22
playerSpawnHeight = 34
candySpawnHeight = 104
carSpawnHeight = 60
floorHeight = 75
playerCapLeft = 65
playerCapRight = 735

-- sprites
candy_green = love.graphics.newImage("candy_blue_800.png")
candy_red = love.graphics.newImage("candy_red_800.png")
candy_yellow = love.graphics.newImage("candy_yellow_800.png")
background = love.graphics.newImage("background_800.png")
character_top = love.graphics.newImage("character_top_800.png")
character_bottom = love.graphics.newImage("character_bottom_800.png")
floor = love.graphics.newImage("floor_800.png")
running_sushi_quad = love.graphics.newQuad(0, 0, 800, 60, 63, 60)
running_sushi = love.graphics.newImage("running_sushi.png")
running_sushi:setWrap("repeat", "repeat")
running_sushi_x = 0
obstacle = love.graphics.newImage("obstacle.png")
obstacle_rail = love.graphics.newImage("obstacle_rail.png")
pipe_cover = love.graphics.newImage("pipe_cover.png")

obstacle_x = love.window.getWidth() * 0.25
obstacle_y = love.window.getHeight() * 0.3
obstacleObject = nil

obstacle_x2 = love.window.getWidth() * 0.75
obstacle_y2 = love.window.getHeight() * 0.5
obstacleObject2 = nil

pipe_red = love.graphics.newImage("pipe_red.png")
pipe_green = love.graphics.newImage("pipe_blue.png")
pipe_yellow = love.graphics.newImage("pipe_yellow.png")

pipe_light = {}
pipe_light[1] = love.graphics.newImage("pipe_light_red.png")
pipe_light[2] = love.graphics.newImage("pipe_light_green.png")
pipe_light[3] = love.graphics.newImage("pipe_light_yellow.png")

pipe_light2 = {}
pipe_light2[1] = love.graphics.newImage("pipe_light_red_2.png")
pipe_light2[2] = love.graphics.newImage("pipe_light_blue_2.png")
pipe_light2[3] = love.graphics.newImage("pipe_light_yellow_2.png")

game_state = 0
candy_wish = nil

function love.load()
	tubeCapClose = {}
	for i = 1, 3 do
		tubeCapClose[i] = false
	end

	love.physics.setMeter(64)
	world = love.physics.newWorld(0, 0, true)
	world:setCallbacks(beginContact, endContact, preSolve, postSolve)

	player = {}
	player.body = love.physics.newBody(world, love.window.getWidth() * 0.5 - 32, love.window.getHeight() - playerSpawnHeight, "kinematic")
	player.shape = love.physics.newRectangleShape(0, 0, playerWidth, playerHeight)
	player.fixture = love.physics.newFixture(player.body, player.shape, 0.1)
	player.body:setFixedRotation(true)
	player.isPush = false
	player.fixture:setUserData("Player")

	-- Border
	CreatePhysicsRect(love.window.getWidth() * 0.5, love.window.getHeight(), love.window.getWidth(), 64)
	CreatePhysicsRect(0, love.window.getHeight(), 64, 128)
	CreatePhysicsRect(love.window.getWidth(), love.window.getHeight(), 64, 128)
	CreatePhysicsRect(love.window.getWidth() * 0.5, 0, love.window.getWidth(), 16)
	CreatePhysicsRect(16, 220, 32, 480)
	CreatePhysicsRect(love.window.getWidth() - 16, 220, 32, 480)
	obstacleObject = CreatePhysicsCircle(obstacle_x, obstacle_y, 28)
	obstacleObject2 = CreatePhysicsCircle(obstacle_x2, obstacle_y2, 28)

	-- Tubes
	tube = {}
	for i = 1, 3 do
		tube[i] = {}
		tube[i][1] = CreatePhysicsRect(love.window.getWidth() * 0.25 * i - 64, 24, 8, 96)
		tube[i][2] = CreatePhysicsRect(love.window.getWidth() * 0.25 * i + 64, 24, 8, 96)
		tube[i][3] = CreatePhysicsRect(love.window.getWidth() * 0.25 * i, 8, 128, 8)
		tube[i][3].fixture:setUserData("Tube" .. (i - 1))
		tube[i][4] = CreatePhysicsRect(love.window.getWidth() * 0.25 * i, 88, 128, 8)
		tube[i][4].body:setActive(false)
	end

	box_count = 0
	box_spawn = love.timer.getTime()
	box = {}
end

function love.update(dt)
	if game_state == 0 then

	elseif game_state == 1 then
		world:update(dt)
		UpdateClient()
		running_sushi_x = running_sushi_x + dt
		obstacle_x = love.window.getWidth() * 0.25 + math.sin(love.timer.getTime()) * 145
		obstacle_x2 = love.window.getWidth() * 0.75 + math.cos(love.timer.getTime()) * 145
		obstacleObject.body:setPosition(obstacle_x, obstacle_y)
		obstacleObject2.body:setPosition(obstacle_x2, obstacle_y2)

		if love.keyboard.isDown("left") or love.keyboard.isDown("a") then
			player.body:setLinearVelocity(-1 * playerMoveSpeed, 0)
		elseif love.keyboard.isDown("right") or love.keyboard.isDown("d") then
			player.body:setLinearVelocity(playerMoveSpeed, 0)
		else
			player.body:setLinearVelocity(0, 0)
		end
		
		local px, py = player.body:getPosition()
		if px <= playerCapLeft then player.body:setX(playerCapLeft) end
		if px >= playerCapRight then player.body:setX(playerCapRight) end

		if (love.keyboard.isDown(" ") or love.keyboard.isDown("up") or love.keyboard.isDown("w")) and not player.isPush then
			player.isPush = love.timer.getTime()
		end

		if player.isPush and player.isPush > love.timer.getTime() - 0.1 then
			player.body:setLinearVelocity(0, -1 * playerJumpPowerUp)
		elseif player.isPush and player.isPush > love.timer.getTime() - 0.2 then
			player.body:setLinearVelocity(0, playerJumpPowerDown)
		elseif player.isPush then
			player.isPush = false
			local x, y = player.body:getPosition()
			player.body:setLinearVelocity(0, 0)
			player.body:setPosition(x, love.window.getHeight() - playerSpawnHeight)
		end

		for i = 1, box_count do
			if box[i] then
				local x, y = box[i].body:getPosition()

				if box[i].bomb and box[i].bomb < love.timer.getTime() - 2 then
					box[i].destroy = true
				end

				if box[i].destroy or x > love.window.getWidth() then
					box[i].body:destroy()
					box[i] = nil
				end
			end
		end

		if box_spawn < love.timer.getTime() - boxSpawnRateInSeconds then
			box_spawn = love.timer.getTime()

			box_count = box_count + math.floor(math.random() * 3) + 1
			box[box_count] = {}
			box[box_count].body = love.physics.newBody(world, -128, love.window.getHeight() - candySpawnHeight, "dynamic")
			box[box_count].shape = love.physics.newCircleShape(0, 0, candyRadius)
			box[box_count].fixture = love.physics.newFixture(box[box_count].body, box[box_count].shape, 10)
			box[box_count].body:setLinearVelocity(boxMovementVelocity, 0)
			box[box_count].fixture:setUserData(box_count)
		end

		for i = 1, 3 do
			if tubeCapClose[i] then
				if tubeCapClose[i] > love.timer.getTime() - 2 then
					tube[i][4].body:setActive(true)
				else
					tube[i][4].body:setActive(false)
					tubeCapClose[i] = false
				end
			end
		end
	end
end

function love.draw()
	if game_state == 0 then
		love.graphics.print("Player 1 (Press 1)", 8, 8)
		love.graphics.print("Player 2 (Press 2)", 8, 32)
	elseif game_state == 1 then
		love.graphics.setColor(255, 255, 255)
		love.graphics.draw(background)
		running_sushi_quad:setViewport(running_sushi_x * -200, 0, 800, 60)
		love.graphics.draw(running_sushi, running_sushi_quad, 0, 466)

		-- Candy
		for i = 1, box_count do
			if box[i] and not box[i].destroy then
				local alpha = 255

				if box[i].bomb then
					alpha = (box[i].bomb - love.timer.getTime()) * 127
				end

				local image = nil
				if i % 3 == 0 then
					image = candy_red
				elseif i % 3 == 1 then
					image = candy_green
				elseif i % 3 == 2 then
					image = candy_yellow
				end
							
				local x, y = box[i].body:getPosition()
				--love.graphics.circle("fill", x, y, candyRadius, 100 )
				local r = box[i].body:getAngle()
				love.graphics.setColor(255, 255, 255, alpha)
				love.graphics.draw(image, x, y, r, 1, 1, candyRadius + 2, candyRadius + 3)
			end
		end

		-- Tubes
		love.graphics.setColor(255, 255, 255)
		for i = 1, 3 do
			if tube[i][4].body:isActive() then
				love.graphics.draw(pipe_cover, love.window.getWidth() * 0.25 * i - 79, 70)
			end
		end

		love.graphics.setColor(255, 255, 255)
		love.graphics.draw(pipe_red, 120, 0)
		love.graphics.draw(pipe_green, 320, 0)
		love.graphics.draw(pipe_yellow, 520, 0)

		if candy_wish then
			if math.floor(love.timer.getTime()) % 2 == 0 then
				--love.graphics.draw(pipe_light[(candy_wish + 1)], love.window.getWidth() * 0.25 * (candy_wish + 1) - 55, 14)
			else
				love.graphics.draw(pipe_light2[(candy_wish + 1)], love.window.getWidth() * 0.25 * (candy_wish + 1) - 64, 6)
			end
		end

		local x, y = player.body:getPosition()
		love.graphics.setColor(255, 255, 255)
		love.graphics.draw(character_top, x, y, 0, 1, 1, playerWidth / 2, playerHeight / 2)

		love.graphics.setColor(255, 255, 255)
		love.graphics.draw(floor, 0, love.window.getHeight() - floorHeight)
		
		local h = love.window.getHeight() - carSpawnHeight
		love.graphics.setColor(255, 255, 255)
		love.graphics.draw(character_bottom, x, h, 0, 1, 1, playerWidth / 2, 0)

		love.graphics.draw(obstacle_rail, love.window.getWidth() * 0.25 - 157, love.window.getHeight() * 0.3 - 10)
		love.graphics.draw(obstacle, obstacle_x - 28, obstacle_y - 28)

		love.graphics.draw(obstacle_rail, love.window.getWidth() * 0.75 - 157, love.window.getHeight() * 0.5 - 10)
		love.graphics.draw(obstacle, obstacle_x2 - 28, obstacle_y2 - 28)
	end
end

function love.keypressed(key)
	if game_state == 0 then
		if key == "1" then
			StartClient(0)
			game_state = 1
		elseif key == "2" then
			StartClient(1)
			game_state = 1
		end
	end
end

function beginContact(a, b, coll)
	local collWall = true

	for i = 1, 3 do
		if a:getUserData() == "Tube" .. (i - 1) then
			if b:getUserData() % 3 == (i - 1) then
				SendMessage(json.encode({res = (i - 1)}))
			else
				tubeCapClose[i] = love.timer.getTime()
			end
			box[b:getUserData()].destroy = true
			collWall = false
		end

		if b:getUserData() == "Tube" .. (i - 1) then
			if b:getUserData() % 3 == (i - 1) then
				SendMessage(json.encode({res = (i - 1)}))
			else
				tubeCapClose[i] = love.timer.getTime()
			end
			box[a:getUserData()].destroy = true
			collWall = false
		end
	end

	if a:getUserData() == "Player" or b:getUserData() == "Player" then
		collWall = false
	end

	if collWall then
		if type(a:getUserData()) == "number" and not box[a:getUserData()].bomb and type(b:getUserData()) ~= "number" then
			box[a:getUserData()].bomb = love.timer.getTime()
		elseif type(b:getUserData()) == "number" and not box[b:getUserData()].bomb and type(a:getUserData()) ~= "number" then
			box[b:getUserData()].bomb = love.timer.getTime()
		end
	end
end

function onServerReceive(data)
	candy_wish = data.wish
end

function love.quit()
	QuitClient()
end

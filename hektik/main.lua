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
candy_green = love.graphics.newImage("candy_green_800.png")
candy_red = love.graphics.newImage("candy_red_800.png")
candy_yellow = love.graphics.newImage("candy_yellow_800.png")
background = love.graphics.newImage("background_800.png")
character_top = love.graphics.newImage("character_top_800.png")
character_bottom = love.graphics.newImage("character_bottom_800.png")
floor = love.graphics.newImage("floor_800.png") 

function love.load()
	tubeCapClose1 = false
	tubeCapClose2 = false
	tubeCapClose3 = false

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
	border = {}
	border.body = love.physics.newBody(world, love.window.getWidth() * 0.5, love.window.getHeight(), "static")
	border.shape = love.physics.newRectangleShape(0, 0, love.window.getWidth(), 64)
	border.fixture = love.physics.newFixture(border.body, border.shape, 5)

	border2 = {}
	border2.body = love.physics.newBody(world, 0, love.window.getHeight(), "static")
	border2.shape = love.physics.newRectangleShape(0, 0, 64, 128)
	border2.fixture = love.physics.newFixture(border2.body, border2.shape, 5)

	border3 = {}
	border3.body = love.physics.newBody(world, love.window.getWidth(), love.window.getHeight(), "static")
	border3.shape = love.physics.newRectangleShape(0, 0, 64, 128)
	border3.fixture = love.physics.newFixture(border3.body, border3.shape, 5)

	border4 = {}
	border4.body = love.physics.newBody(world, love.window.getWidth() * 0.5, 0, "static")
	border4.shape = love.physics.newRectangleShape(0, 0, love.window.getWidth(), 16)
	border4.fixture = love.physics.newFixture(border4.body, border4.shape, 5)

	border5 = {}
	border5.body = love.physics.newBody(world, 16, 220, "static")
	border5.shape = love.physics.newRectangleShape(0, 0, 32, 480)
	border5.fixture = love.physics.newFixture(border5.body, border5.shape, 5)

	border6 = {}
	border6.body = love.physics.newBody(world, love.window.getWidth() - 16, 220, "static")
	border6.shape = love.physics.newRectangleShape(0, 0, 32, 480)
	border6.fixture = love.physics.newFixture(border6.body, border6.shape, 5)

	-- Tubes 1
	tubeBorder1 = {}
	tubeBorder1.body = love.physics.newBody(world, love.window.getWidth() * 0.25 - 64, 24, "static")
	tubeBorder1.shape = love.physics.newRectangleShape(0, 0, 8, 32)
	tubeBorder1.fixture = love.physics.newFixture(tubeBorder1.body, tubeBorder1.shape, 5)

	tubeBorder2 = {}
	tubeBorder2.body = love.physics.newBody(world, love.window.getWidth() * 0.25 + 64, 24, "static")
	tubeBorder2.shape = love.physics.newRectangleShape(0, 0, 8, 32)
	tubeBorder2.fixture = love.physics.newFixture(tubeBorder2.body, tubeBorder2.shape, 5)

	tubeEnd1 = {}
	tubeEnd1.body = love.physics.newBody(world, love.window.getWidth() * 0.25, 8, "static")
	tubeEnd1.shape = love.physics.newRectangleShape(0, 0, 128, 8)
	tubeEnd1.fixture = love.physics.newFixture(tubeEnd1.body, tubeEnd1.shape, 5)
	tubeEnd1.fixture:setUserData("Tube0")

	tubeCap1 = {}
	tubeCap1.body = love.physics.newBody(world, love.window.getWidth() * 0.25, 32, "static")
	tubeCap1.shape = love.physics.newRectangleShape(0, 0, 128, 8)
	tubeCap1.fixture = love.physics.newFixture(tubeCap1.body, tubeCap1.shape, 5)
	tubeCap1.body:setActive(false)

	-- Tubes 2
	tubeBorder3 = {}
	tubeBorder3.body = love.physics.newBody(world, love.window.getWidth() * 0.5 - 64, 24, "static")
	tubeBorder3.shape = love.physics.newRectangleShape(0, 0, 8, 32)
	tubeBorder3.fixture = love.physics.newFixture(tubeBorder3.body, tubeBorder3.shape, 5)

	tubeCap2 = {}
	tubeCap2.body = love.physics.newBody(world, love.window.getWidth() * 0.5, 32, "static")
	tubeCap2.shape = love.physics.newRectangleShape(0, 0, 128, 8)
	tubeCap2.fixture = love.physics.newFixture(tubeCap2.body, tubeCap2.shape, 5)
	tubeCap2.body:setActive(false)

	tubeBorder4 = {}
	tubeBorder4.body = love.physics.newBody(world, love.window.getWidth() * 0.5 + 64, 24, "static")
	tubeBorder4.shape = love.physics.newRectangleShape(0, 0, 8, 32)
	tubeBorder4.fixture = love.physics.newFixture(tubeBorder4.body, tubeBorder4.shape, 5)

	tubeCap3 = {}
	tubeCap3.body = love.physics.newBody(world, love.window.getWidth() * 0.75, 32, "static")
	tubeCap3.shape = love.physics.newRectangleShape(0, 0, 128, 8)
	tubeCap3.fixture = love.physics.newFixture(tubeCap3.body, tubeCap3.shape, 5)
	tubeCap3.body:setActive(false)

	tubeEnd2 = {}
	tubeEnd2.body = love.physics.newBody(world, love.window.getWidth() * 0.5, 8, "static")
	tubeEnd2.shape = love.physics.newRectangleShape(0, 0, 128, 8)
	tubeEnd2.fixture = love.physics.newFixture(tubeEnd2.body, tubeEnd2.shape, 5)
	tubeEnd2.fixture:setUserData("Tube1")

	-- Tubes 3
	tubeBorder5 = {}
	tubeBorder5.body = love.physics.newBody(world, love.window.getWidth() * 0.75 - 64, 24, "static")
	tubeBorder5.shape = love.physics.newRectangleShape(0, 0, 8, 32)
	tubeBorder5.fixture = love.physics.newFixture(tubeBorder5.body, tubeBorder5.shape, 5)

	tubeBorder6 = {}
	tubeBorder6.body = love.physics.newBody(world, love.window.getWidth() * 0.75 + 64, 24, "static")
	tubeBorder6.shape = love.physics.newRectangleShape(0, 0, 8, 32)
	tubeBorder6.fixture = love.physics.newFixture(tubeBorder6.body, tubeBorder6.shape, 5)

	tubeEnd3 = {}
	tubeEnd3.body = love.physics.newBody(world, love.window.getWidth() * 0.75, 8, "static")
	tubeEnd3.shape = love.physics.newRectangleShape(0, 0, 128, 8)
	tubeEnd3.fixture = love.physics.newFixture(tubeEnd3.body, tubeEnd3.shape, 5)
	tubeEnd3.fixture:setUserData("Tube2")

	box_count = 0
	box_spawn = love.timer.getTime()
	box = {}
end

function love.update(dt)
	world:update(dt)

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

	if love.keyboard.isDown(" ") and not player.isPush then
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

	if tubeCapClose1 then
		if tubeCapClose1 > love.timer.getTime() - 2 then
			tubeCap1.body:setActive(true)
		else
			tubeCap1.body:setActive(false)
			tubeCapClose1 = false
		end
	end

	if tubeCapClose2 then
		if tubeCapClose2 > love.timer.getTime() - 2 then
			tubeCap2.body:setActive(true)
		else
			tubeCap2.body:setActive(false)
			tubeCapClose2 = false
		end
	end

	if tubeCapClose3 then
		if tubeCapClose3 > love.timer.getTime() - 2 then
			tubeCap3.body:setActive(true)
		else
			tubeCap3.body:setActive(false)
			tubeCapClose3 = false
		end
	end
end

function love.draw()
	love.graphics.draw(background)

	love.graphics.setColor(255, 255, 255)
	-- Tube 1
	love.graphics.polygon("fill", tubeBorder1.body:getWorldPoints(tubeBorder1.shape:getPoints()))
	love.graphics.polygon("fill", tubeBorder2.body:getWorldPoints(tubeBorder2.shape:getPoints()))
	love.graphics.setColor(255, 0, 0)
	love.graphics.polygon("fill", tubeEnd1.body:getWorldPoints(tubeEnd1.shape:getPoints()))
	if tubeCap1.body:isActive() then
		love.graphics.setColor(255, 255, 255)
		love.graphics.polygon("fill", tubeCap1.body:getWorldPoints(tubeCap1.shape:getPoints()))
	end

	love.graphics.setColor(255, 255, 255)
	-- Tube 2
	love.graphics.polygon("fill", tubeBorder3.body:getWorldPoints(tubeBorder1.shape:getPoints()))
	love.graphics.polygon("fill", tubeBorder4.body:getWorldPoints(tubeBorder2.shape:getPoints()))
	love.graphics.setColor(0, 255, 0)
	love.graphics.polygon("fill", tubeEnd2.body:getWorldPoints(tubeEnd2.shape:getPoints()))
	if tubeCap2.body:isActive() then
		love.graphics.setColor(255, 255, 255)
		love.graphics.polygon("fill", tubeCap2.body:getWorldPoints(tubeCap2.shape:getPoints()))
	end

	love.graphics.setColor(255, 255, 255)
	-- Tube 3
	love.graphics.polygon("fill", tubeBorder5.body:getWorldPoints(tubeBorder1.shape:getPoints()))
	love.graphics.polygon("fill", tubeBorder6.body:getWorldPoints(tubeBorder2.shape:getPoints()))
	love.graphics.setColor(255, 255, 0)
	love.graphics.polygon("fill", tubeEnd3.body:getWorldPoints(tubeEnd3.shape:getPoints()))
	if tubeCap3.body:isActive() then
		love.graphics.setColor(255, 255, 255)
		love.graphics.polygon("fill", tubeCap3.body:getWorldPoints(tubeCap3.shape:getPoints()))
	end

	love.graphics.setColor(255, 255, 255)
	
	-- no need to see those anymore
	--love.graphics.polygon("fill", player.body:getWorldPoints(player.shape:getPoints()))
	--[[love.graphics.polygon("fill", border.body:getWorldPoints(border.shape:getPoints()))
	love.graphics.polygon("fill", border2.body:getWorldPoints(border2.shape:getPoints()))
	love.graphics.polygon("fill", border3.body:getWorldPoints(border3.shape:getPoints()))
	love.graphics.polygon("fill", border4.body:getWorldPoints(border4.shape:getPoints()))
	love.graphics.polygon("fill", border5.body:getWorldPoints(border5.shape:getPoints()))
	love.graphics.polygon("fill", border6.body:getWorldPoints(border6.shape:getPoints()))]]--

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
			love.graphics.draw(image, x, y, r, 1, 1, candyRadius + 2, candyRadius + 3)
		end
	end
	
	local x, y = player.body:getPosition()
	love.graphics.draw(character_top, x, y, 0, 1, 1, playerWidth / 2, playerHeight / 2)
	
	love.graphics.draw(floor, 0, love.window.getHeight() - floorHeight)
	
	local h = love.window.getHeight() - carSpawnHeight
	love.graphics.draw(character_bottom, x, h, 0, 1, 1, playerWidth / 2, 0)
end

function beginContact(a, b, coll)
	local collWall = true

	if a:getUserData() == "Tube0" then
		if b:getUserData() % 3 == 0 then
			print("Tube1")
		else
			tubeCapClose1 = love.timer.getTime()
		end
		box[b:getUserData()].destroy = true
		collWall = false
	end

	if b:getUserData() == "Tube0" then
		if b:getUserData() % 3 == 0 then
			print("Tube1")
		else
			tubeCapClose1 = love.timer.getTime()
		end
		box[a:getUserData()].destroy = true
		collWall = false
	end

	if a:getUserData() == "Tube1" then
		if b:getUserData() % 3 == 1 then
			print("Tube2")
		else
			tubeCapClose2 = love.timer.getTime()
		end
		box[b:getUserData()].destroy = true
		collWall = false
	end

	if b:getUserData() == "Tube1" then
		if b:getUserData() % 3 == 1 then
			print("Tube2")
		else
			tubeCapClose2 = love.timer.getTime()
		end
		box[a:getUserData()].destroy = true
		collWall = false
	end

	if a:getUserData() == "Tube2" then
		if b:getUserData() % 3 == 2 then
			print("Tube3")
		else
			tubeCapClose3 = love.timer.getTime()
		end
		box[b:getUserData()].destroy = true
		collWall = false
	end

	if b:getUserData() == "Tube2" then
		if b:getUserData() % 3 == 2 then
			print("Tube3")
		else
			tubeCapClose3 = love.timer.getTime()
		end
		box[a:getUserData()].destroy = true
		collWall = false
	end

	-- i guess this is not used anymore // alex
	--[[if a:getUserData() == "Player" then
		box[b:getUserData()].body:setLinearVelocity(0, 0)
		collWall = false
	end

	if b:getUserData() == "Player" then
		box[a:getUserData()].body:setLinearVelocity(0, 0)
		collWall = false
	end]]--

	if collWall then
		if type(a:getUserData()) == "number" then
			box[a:getUserData()].bomb = love.timer.getTime()
		elseif type(b:getUserData()) == "number" then
			box[b:getUserData()].bomb = love.timer.getTime()
		end
	end
end

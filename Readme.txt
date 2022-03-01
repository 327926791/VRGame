Implemetation of VR game
1.1.1 Enemy Charater
Implemented enemy character with a red texture. Attach Gun script and a new Enemy script to enmey characters. The Gun script use for dectecting and shoot at player, as well as processing hit damage. The Enemy script use for controll the movement, such as patrolling along the path we set and moving towards player.

1.1.2 Detecting Player
When the player is within 15 meters to the enemy and also in enmey's viewing angle of 135 degrees, the enemy will detect player. Then enemy will turn and move towards player and keep a distance within 2 - 10 meters

1.1.3 Enemy Shooting The Player
When enemy detected player, it will start shooting at player if player is within 10 meters from enemy and player in the viewing angel of 45 degree from enemy, and there's no wall between. The enemy have a same shooting rate at what the player can do. A Gaussian variation is added to enmey's shooting vector, so it will result in the lower accuracy if player is far from enemy but a higher accuracy is near. When Player died it will play death animation and move the camera to a fixed position

1.1.4 Player Shooting The Enemy And Health
Player can shoot and kill enemy. When enemy died, his gun will dropped on the ground. Player's health is showed on the UI, below the ammunition infomation. Enemy will detect player immediately if being shot by player. If there're multiple enemies in the same room, shoot at one enemy will get alerted by other enemies.

1.1.5 Game Environment
The game has 3 rooms, with 1 enmey in the first room, 2 enemies in the second and 3 enemies in the third. The third room has a little escape room guarded by an enemy. shooting at the guard will alerted the other 2 enemies patrolling around but if player shoot the patrolling enmeies, the guard won't be alerted if player is not in his viewing range. Player will win the game when entering the escape room even if you don't kill all the enemies. So if you run fast enough and lucky enough, you can win the game without firing once.

1.2 Bouns
1.2.1 Ammunition Supply
Instead of creating ammo crates to refill ammunition, player can loot from the guns dropping by enemies. It will refill player's remaining bullets to a max of 90 when player reach the dropping guns.

1.2.3 Dectecting Body Part Hit.
Multiple bounding box was add to all body parts. Hitting hands, chest, legs and head will cause 10,30,20,100 damage per hit.


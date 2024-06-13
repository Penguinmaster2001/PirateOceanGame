
extends Node3D


var speed := 0.4

var rockage := 9.0
var rock_offset := 0.0
var rock_speed := 1.0

var rollage := 5.0
var roll_offset := 0.0
var roll_speed := 1.0

var time_elapsed := 0.0

var look_at_target := Vector3.ZERO



func _ready() -> void:
    var rng := RandomNumberGenerator.new()

    rock_speed = rng.randf_range(0.8, 1.2)
    roll_speed = rng.randf_range(0.8, 1.2)

    rock_offset = rng.randf() * rock_speed
    roll_offset = rng.randf() * roll_speed



func _process(delta: float) -> void:
    var prev_y_rot := rotation.y

    if position != look_at_target:
        look_at(look_at_target)
    
    var y_rot_target := rotation.y

    rotation.y = rotate_toward(prev_y_rot, y_rot_target, delta * 10.0)
    rotation.x = deg_to_rad(rockage * sin(2.0 * PI * time_elapsed * speed * rock_speed + rock_offset))
    rotation.z = deg_to_rad(rollage * sin(2.0 * PI * time_elapsed * speed * roll_speed + roll_offset))

    time_elapsed += delta




func update_look_at_target(new_target: Vector3) -> void:
    look_at_target = new_target

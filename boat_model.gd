
extends Node3D


@export_range (0.0, 10.0) var speed: float

@export_range (0.0, 20.0) var rockage: float
@export_range (0.0, 20.0) var rollage: float

var time_elapsed := 0.0

var look_at_target := Vector3.ZERO



func _process(delta: float) -> void:
    var prev_y_rot := rotation.y

    if position != look_at_target:
        look_at(look_at_target)
    
    var y_rot_target := rotation.y

    rotation.y = rotate_toward(prev_y_rot, y_rot_target, delta * 10.0)
    rotation.x = deg_to_rad(rockage * sin(time_elapsed * speed))
    rotation.z = deg_to_rad(rollage * sin(time_elapsed * speed))




func update_look_at_target(new_target: Vector3) -> void:
    look_at_target = new_target


extends Node3D


@export_range (0.0, 10.0) var speed: float

@export_range (0.0, 20.0) var rockage: float
@export_range (0.0, 20.0) var rollage: float

var time_elapsed := 0.0

var look_at_target := Vector3.ZERO



func _process(delta: float) -> void:
    time_elapsed += delta

    var dir := position.direction_to(look_at_target)

    # print(rotation.y)
    # print(atan2(dir.z, dir.x))
    # print(rotate_toward(rotation.y, atan2(dir.x, dir.z), 20.0))
    # print(look_at_target)
    print()

    # self.rotation_degrees = Vector3(rockage * sin(time_elapsed * speed), \
    #     rad_to_deg(atan2(dir.x, dir.z)), \
    #     rollage * sin(time_elapsed * speed))

    var prev_y_rot := rotation.y
    look_at(look_at_target)
    var y_rot_target := rotation.y

    rotation.y = rotate_toward(prev_y_rot, y_rot_target, delta * 10.0)
    rotation.x = deg_to_rad(rockage * sin(time_elapsed * speed))
    rotation.z = deg_to_rad(rollage * sin(time_elapsed * speed))




func update_look_at_target(new_target: Vector3) -> void:
    look_at_target = new_target

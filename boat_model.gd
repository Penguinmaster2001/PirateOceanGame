
extends Node3D


@export_range (0.0, 10.0) var speed

@export_range (0.0, 20.0) var rockage
@export_range (0.0, 20.0) var rollage

var time_elapsed = 0.0

func _process(delta):
    time_elapsed += delta
    self.rotation_degrees = Vector3(rockage * sin(time_elapsed * speed), 0.0, rollage * sin(time_elapsed * speed))

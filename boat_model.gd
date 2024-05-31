
extends Node3D


@export_range (0.0, 10.0) var speed: float

@export_range (0.0, 20.0) var rockage: float
@export_range (0.0, 20.0) var rollage: float

var time_elapsed := 0.0



func _process(delta: float) -> void:
    time_elapsed += delta
    self.rotation_degrees = Vector3(rockage * sin(time_elapsed * speed), 0.0, rollage * sin(time_elapsed * speed))

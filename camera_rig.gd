
extends Node3D

var right_click_pressed := false

var maxPitchDeg := 30.0
var minPitchDeg := -45.0
var maxZoom := 1000.0
var minZoom := 6.0
var zoomStep := 2.0
var verticalSensitivity := 0.002
var horizontalSensitivity := 0.002

var _curZoom : float = maxZoom



func _process(_delta: float) -> void:
    get_child(0).position = Vector3(0.0, _curZoom, _curZoom)



func _unhandled_input(event: InputEvent) -> void:
    if event is InputEventMouseButton:
        if event.button_index == MOUSE_BUTTON_RIGHT:
            right_click_pressed = event.pressed
        
        if event.button_index == MOUSE_BUTTON_WHEEL_UP and _curZoom > minZoom:
            _curZoom -= zoomStep
            
        if event.button_index == MOUSE_BUTTON_WHEEL_DOWN and _curZoom < maxZoom:
            _curZoom += zoomStep


    elif event is InputEventMouseMotion and right_click_pressed:
        rotation.y -= event.relative.x * horizontalSensitivity
        rotation.y = wrapf(rotation.y,0.0,TAU)
		
        rotation.x -= event.relative.y * verticalSensitivity
        rotation.x = clamp(rotation.x, deg_to_rad(minPitchDeg), deg_to_rad(maxPitchDeg))

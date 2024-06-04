
extends Node3D

var right_click_pressed := false

var max_pitch_deg := 30.0
var min_pitch_deg := -45.0
var max_zoom := 3000.0
var min_zoom := 6.0
var zoom_step := 1.0
var vertical_sensitivity := 0.002
var horizontal_sensitivity := 0.002

var _cur_zoom : float = max_zoom



func _process(_delta: float) -> void:
    get_child(0).position = Vector3(0.0, _cur_zoom, _cur_zoom)



func _unhandled_input(event: InputEvent) -> void:
    if event is InputEventMouseButton:
        if event.button_index == MOUSE_BUTTON_RIGHT:
            right_click_pressed = event.pressed
        
        var zoom_factor := clampf(_cur_zoom / 50.0, 0.5, 50.0)

        if event.button_index == MOUSE_BUTTON_WHEEL_UP and _cur_zoom > min_zoom:
            _cur_zoom -= zoom_step * zoom_factor
            
        if event.button_index == MOUSE_BUTTON_WHEEL_DOWN and _cur_zoom < max_zoom:
            _cur_zoom += zoom_step * zoom_factor
        
        _cur_zoom = clampf(_cur_zoom, min_zoom, max_zoom)


    elif event is InputEventMouseMotion and right_click_pressed:
        rotation.y -= event.relative.x * horizontal_sensitivity
        rotation.y = wrapf(rotation.y,0.0,TAU)
		
        rotation.x -= event.relative.y * vertical_sensitivity
        rotation.x = clamp(rotation.x, deg_to_rad(min_pitch_deg), deg_to_rad(max_pitch_deg))

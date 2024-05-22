
extends Node
# class_name HexMap

var num_hexes = 0
var hex_storage = { }
var hex_list = [ ]



# func _init(size: int):
# 	_generate_triangle(size)


func _add_hex(q, r):
	# if not hex_storage.has([q, r]):
	#     num_hexes += 1

	num_hexes += 1
	
	var new_hex = Hex.new(q, r)

	hex_storage[[q, r]] = new_hex
	hex_list.append(new_hex)



func _generate_parallelogram(l: int, w: int):
	for q in range(l):
		for r in range(w):
			_add_hex(q, r)



func generate_triangle(size: int):
	for q in range(size):
		for r in range(size - q):
			_add_hex(q, r)
	
	print(hex_list)



func _generate_hexagon(radius: int):
	for q in range(-radius, radius + 1):
		var r1 = max(-radius, -q - radius)
		var r2 = min( radius, -q + radius)

		for r in range(r1, r2 + 1):
			_add_hex(q, r)



func get_hex(q, r):
	return hex_storage.get([q, r], null)



func get_hexes():
	return hex_list

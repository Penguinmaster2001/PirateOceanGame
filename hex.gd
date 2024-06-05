
extends Node3D

class_name Hex

# This contains simple operations on the hexagon grid

# Right now this only really does stuff with Vector3i
# But it should be able to take in hex_tiles for some operations eventually

static var directions := [
	Hex.new( 1,  0), # Right
	Hex.new( 1, -1), # Up Right
	Hex.new( 0, -1), # Up Left
	Hex.new(-1,  0), # Left
	Hex.new(-1,  1), # Down Left
	Hex.new( 0,  1)  # Down Right
]



# Cube coords
var _q: int
var _r: int
var _s: int

# This class will eventually need to include other stuff such as
# Terrain type
# model
# other stuff idk

var terrain_type : int = 0



func _init(init_q: int, init_r: int) -> void:
	self._q = init_q
	self._r = init_r
	self._s = -init_q - init_r



func get_world_coords(size: float) -> Vector3:
	# Pointy
	var x := size * ((sqrt(3.0) * self._q)  +  ((sqrt(3.0) / 2.0) * self._r))
	var y := size * 1.5 * self._r
	return Vector3(x, 0, y)


func get_mag() -> int:
	return int((abs(self._q) + abs(self._r) + abs(self._s)) / 2)

func get_distance(other: Hex) -> int:
	return self.sub(other).get_mag()



func add(other: Hex) -> Hex:
	return Hex.new(self._q + other._q, self._r + other._r)

func sub(other: Hex) -> Hex:
	return Hex.new(self._q - other._q, self._r - other._r)

func scale(f: int) -> Hex:
	return Hex.new(self._q * f, self._r * f)

func hex_lerp(other: Hex, t: float) -> Hex:
	return Hex.hex_coord_round(lerpf(self._q, other._q, t), lerpf(self._r, other._r, t))



static func hex_coord_lerp(q_i: float, r_i: float, q_f: float, r_f: float, t: float) -> Array:
	return [lerp(q_i, q_f, t), lerp(r_i, r_f, t)]



static func hex_coord_round(q_i: float, r_i: float) -> Hex:
	var q := roundi(q_i)
	var r := roundi(r_i)
	var s := -q - r

	var q_diff := absf(q - float(q_i))
	var r_diff := absf(r - float(r_i))
	var s_diff := absf(s - float(-q_i - r_i))
	
	if q_diff > r_diff and q_diff > s_diff:
		q = -r - s
	elif r_diff > s_diff:
		r = -q - s

	return Hex.new(q, r)



# static func hex_coord_mag(q: float, r: float) -> float:
# 	var s := -q - r

# 	return (abs(q) + abs(r) + abs(s)) / 2.0



# static func hex_coord_sub(q_i: float, r_i: float, q_f: float, r_f: float) -> Array:
# 	return [q_i - q_f, r_i - r_f]



# static func hex_coord_dist(q_i: float, r_i: float, q_f: float, r_f: float) -> float:
# 	var subtracted := hex_coord_sub(q_i, r_i, q_f, r_f)

# 	return hex_coord_mag(subtracted[0], subtracted[1])


func get_neighbor_coord(direction: int) -> Hex:
	return self.add(directions[direction % 6])



func get_terrain_type() -> int:
	return terrain_type



func eq(other: Hex) -> bool:
	return self._q == other._q && self._r == other._r



func _to_string() -> String:
	return "Hex(_q: %d, _r: %d, _s: %d)" % [self._q, self._r, self._s]

extends Hex

class_name WfcHex

var _collapsed := false

var _edges : Array
var _allowed_types : Array

# Array of arrays for each edge that contain the allowed adjacent edges
var _allowed_edge_types : Array

var _constraint := 0



func _init(init_q: int, init_r: int) -> void:
	super(init_q, init_r)



# Return the number of types this hexagon can be
func get_constraint() -> int:
	return _constraint



func set_allowed_types(new_allowed_types: Array) -> void:
	_allowed_types = new_allowed_types.duplicate()

	_allowed_edge_types.clear()

	for i in range(6):
		_allowed_edge_types.append([0, -2, -1, 1, 2, 3])

	_constrain()

		# _update_edge_allowed_types(i)



func set_edges(new_edges: Array) -> void:
	_edges = new_edges



# Get an edge
func get_edge(edge: int) -> int:
	return _edges[edge % 6]



func get_allowed_edge_types(edge: int) -> Array:
	return _allowed_edge_types[edge % 6]



# Update the constraint
func _constrain() -> void:
	_constraint = 0

	var allowed_type_weight := 0
	for allowed_type: int in _allowed_types:
		allowed_type_weight += HexMap.type_weights[allowed_type]

	var num_allowed_edges := 0
	for edge: Array in _allowed_edge_types:
		for _type: int in edge:
			num_allowed_edges += 1

	_constraint = mini(36 * allowed_type_weight / 3, num_allowed_edges)



# remove types that don't match the edges, 0 is a wildcard
func constrain_edge(edge: int, allowed_edge_types: Array) -> void:
	edge %= 6

	if allowed_edge_types.has(0):
		return
	
	for allowed_type: int in _allowed_types.duplicate():
		var type_edge : int = HexMap.type_edges[allowed_type][edge]
		
		if not allowed_edge_types.has(type_edge):
			_allowed_types.erase(allowed_type)

	_update_edge_allowed_types(edge)
	
	_constrain()



func _update_edge_allowed_types(edge: int) -> void:
	edge %= 6

	var edge_allowed_edges := [ ]

	for allowed_type: int in _allowed_types:
		var type_edges : Array = HexMap.type_edges[allowed_type]


		if not edge_allowed_edges.has(type_edges[edge]):
			edge_allowed_edges.append(type_edges[edge])

		if edge_allowed_edges.size() == 6:
			break

	_allowed_edge_types[edge] = edge_allowed_edges

	_constrain()




func get_random_allowed_type() -> int:
	if _allowed_types.is_empty():
		return 0

	var cum_weight := 0
	var cum_weights := [ ]

	# Calculate cumulative weights
	for allowed_type: int in _allowed_types:
		cum_weight += HexMap.type_weights[allowed_type]
		cum_weights.append(cum_weight)

	var choice := randi_range(0, cum_weight)

	# Find the lowest index greater than choice
	var index := cum_weights.bsearch(choice)

	return _allowed_types[index]



func collapse(type: int) -> void:
	_collapsed = true
	terrain_type = type
	_edges = HexMap.type_edges[type]



func is_collapsed() -> bool:
	return _collapsed



func _to_string() -> String:
	# var allowed_type_names := [ ]

	# for type: int in _allowed_types:
	# 	allowed_type_names.append(HexMap.type_names[type])

	# return "\n---\nWfcHex(_q: %d, _r: %d)\n%s\n%s\n%s\n%s\n" % [self._q, self._r, self._allowed_types, allowed_type_names, self._collapsed, HexMap.type_names[self.terrain_type]]
	# return "\n---\nWfcHex(_q: %d, _r: %d)\n%s\n%s\n%s\n" % [self._q, self._r, self._allowed_edge_types, self._collapsed, HexMap.type_names[self.terrain_type]]
	return "WfcHex(q: %d, r: %d, traversable: %s)" % [self._q, self._r, HexMap.type_traversable[self.terrain_type]]
	
using Godot;


/* 
 * 
 * 
 */
public partial class Hex : Node3D
{
	private int q;
	public int get_q() => q;

	private int r;
	public int get_r() => r;

	private int s;
	public int get_s() => s;

	
	protected int terrain_type;
	public int get_terrain_type() => terrain_type;

	private static float hex_size = 25.0f;

	public static void set_hex_size(float new_size)
	{
		if (new_size > 0.0f)
			hex_size = new_size;
	}

	public static float get_hex_size()
	{
		return hex_size;
	}


	public Hex(int q, int r)
	{
		this.q = q;
		this.r = r;
		this.s = -q - r;
	}


	public Vector3 get_world_coords()
	{
		float x = hex_size * ((q * Mathf.Sqrt(3.0f)) + (r * Mathf.Sqrt(3.0f) / 2.0f));
		float z = hex_size * 1.5f * r;

		return new Vector3(x, 0.0f, z);
	}


	public static Hex world_coords_to_hex(float x, float y)
	{
		float q_frac = ((x * Mathf.Sqrt(3.0f) / 3.0f) - (y / 3.0f)) / hex_size;
		float r_frac = (y * 2.0f / 3.0f) / hex_size;

		return round_coords(q_frac, r_frac);
	}



	public int get_mag()
	{
		return (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) / 2;
	}



	public int get_distance(Hex other)
	{
		return this.subtract(other).get_mag();
	}



	public float get_world_distance(Hex other)
	{
		return this.get_world_coords().DistanceTo(other.get_world_coords());
	}



	public Hex add(Hex other)
	{
		return new Hex(this.q + other.q, this.r + other.r);
	}



	public Hex subtract(Hex other)
	{
		return new Hex(this.q - other.q, this.r - other.r);
	}



	public static Hex lerp(Hex start, Hex end, float weight)
	{
		return round_coords(Mathf.Lerp(start.q, end.q, weight),
							Mathf.Lerp(start.r, end.r, weight));
	}



	public static Hex round_coords(float q_frac, float r_frac)
	{
		int q = Mathf.RoundToInt(q_frac);
		int r = Mathf.RoundToInt(r_frac);
		int s = -q - r;

		float q_diff = Mathf.Abs(q - q_frac);
		float r_diff = Mathf.Abs(r - r_frac);
		float s_diff = Mathf.Abs(-q_frac - r_frac);

		if (q_diff > r_diff && q_diff > s_diff)
			q = -r - s;
		else if (r_diff > s_diff)
			r = -q - s;

		
		return new Hex(q, r);
	}

	/*
static func hex_coord_lerp(q_i: float, r_i: float, q_f: float, r_f: float, t: float) -> Array:
	return [lerp(q_i, q_f, t), lerp(r_i, r_f, t)]
	*/
}

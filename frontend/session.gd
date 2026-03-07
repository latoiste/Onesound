class_name Session
extends Node

enum Type {
	AVAILABLE,
	REGISTERED,
}

class Details:
	var display_name: String
	var icon: Texture2D
	var registered: bool = false

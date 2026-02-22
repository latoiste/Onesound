extends Node2D

@onready var http_handler: HttpHandler = $HttpHandler

func _ready() -> void:
	await get_tree().create_timer(1.0).timeout
	http_handler.request_media_delete("Chrome")

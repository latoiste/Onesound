class_name Main
extends Control

@onready var http_handler: HttpHandler = $HttpHandler
@onready var request_timer: Timer = $RequestTimer
@onready var available_media_list: MediaList = %AvailableMediaList
@onready var registered_media_list: MediaList = %RegisteredMediaList

var is_requesting: bool = false

func _ready() -> void:
	request_timer.timeout.connect(request_media_sessons)
	request_timer.start(3.0)
	request_timer.one_shot = false
	
	available_media_list.switch.connect(on_media_list_switch)
	available_media_list.switch.connect(
		func(aumid: String, _type: Session.Type): 
			http_handler.request_media_post(aumid))
	
	registered_media_list.switch.connect(on_media_list_switch)
	registered_media_list.switch.connect(
		func(aumid: String, _type: Session.Type): 
			http_handler.request_media_delete(aumid))

func request_media_sessons():
	var result: Array
	
	if is_requesting == true:
		return;
	
	is_requesting = true
	result = await http_handler.request_media_get()
	is_requesting = false
	
	available_media_list.reset_list()
	for r: Dictionary in result:
		var aumid: String = r.get("aumid")
		
		if !registered_media_list.has_aumid(aumid):
			var icon = construct_image_from_base64(r.get("base64EncodedIcon"))
			var media_display := MediaDisplay.new_media_display(aumid, icon, Session.Type.AVAILABLE)
			available_media_list.new_display(aumid, media_display)

func on_media_list_switch(aumid: String, type: Session.Type) -> void:
	var from: MediaList
	var to: MediaList
	
	if type == Session.Type.AVAILABLE:
		from = available_media_list
		to = registered_media_list
	else:
		from = registered_media_list
		to = available_media_list
	
	var media_display = from.remove_display(aumid)
	to.new_display(aumid, media_display)

func construct_image_from_base64(base64String: String) -> ImageTexture:
	var image_texture: ImageTexture
	var bytes := Marshalls.base64_to_raw(base64String)
	var image := Image.new()
	var error := image.load_png_from_buffer(bytes)
	
	if error == OK:
		image_texture = ImageTexture.create_from_image(image)
	
	return image_texture

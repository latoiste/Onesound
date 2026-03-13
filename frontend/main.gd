class_name Main
extends Control

@onready var http_handler: HttpHandler = $HttpHandler
@onready var request_timer: Timer = $RequestTimer
@onready var available_media_list: MediaList = %AvailableMediaList
@onready var registered_media_list: MediaList = %RegisteredMediaList

func _ready() -> void:
	await start_backend()

	await load_registered_aumid()
	
	available_media_list.switch.connect(on_media_list_switch)
	available_media_list.switch.connect(
		func(aumid: String, _type: Session.Type): 
			http_handler.request_media_post(aumid))
	
	registered_media_list.switch.connect(on_media_list_switch)
	registered_media_list.switch.connect(
		func(aumid: String, _type: Session.Type): 
			http_handler.request_media_delete(aumid))
	
	request_media_sessions()
	request_timer.timeout.connect(request_media_sessions)
	request_timer.start(3.0)
	request_timer.one_shot = false

func request_media_sessions():
	var result: Array = await http_handler.request_media_get()
	
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
	if base64String == "":
		return null
	var image_texture: ImageTexture
	var bytes := Marshalls.base64_to_raw(base64String)
	var image := Image.new()
	var error := image.load_png_from_buffer(bytes)
	
	if error == OK:
		image_texture = ImageTexture.create_from_image(image)
	
	return image_texture

func start_backend() -> void:
	var user_os: String = OS.get_name()
	var running: bool = false
	var backend_path: String
	
	match user_os:
		"Windows":
			backend_path = ProjectSettings.globalize_path("res://bin/windows/OneSound.exe")
		"Linux":
			pass
		"macOS":
			pass
		_:
			print("OS is not supported")
			return
	
	running = await http_handler.check_backend_health()
	if !running:
		OS.create_process(backend_path, [], true)
		await http_handler.check_backend_health()
	else:
		print("backend is already running")

func load_registered_aumid() -> void:
	var result: Array = await http_handler.request_media_get(true)
	
	for r: Dictionary in result:
		var aumid: String = r.get("aumid")
		var icon = construct_image_from_base64(r.get("base64EncodedIcon"))
		var media_display := MediaDisplay.new_media_display(aumid, icon, Session.Type.REGISTERED)
		
		registered_media_list.new_display(aumid, media_display)

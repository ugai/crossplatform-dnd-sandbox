use iced::widget::{button, column, container, row, text};
use iced::{event, mouse, window, Element, Event, Point};
use iced::{Size, Subscription};
use rfd::FileDialog;
use std::path::PathBuf;

pub fn main() -> iced::Result {
    iced::application("Drag and drop", update, view)
        .window_size(Size::new(600.0, 200.0))
        .subscription(subscription)
        .run()
}

fn update(state: &mut State, message: Message) {
    match message {
        Message::FileDialogRequest => {
            if let Some(path) = FileDialog::new().pick_file() {
                state.file_path = Some(path);
            };
        }
        Message::FileDropped(path) => state.file_path = Some(path),
        Message::MouseMoved(position) => state.mouse_position = Some(position),
    }
}

fn subscription(_state: &State) -> Subscription<Message> {
    event::listen_with(|event, _status, _windows| match event {
        Event::Window(window::Event::FileDropped(path)) => Some(Message::FileDropped(path)),
        Event::Mouse(mouse::Event::CursorMoved { position }) => Some(Message::MouseMoved(position)),
        _ => None,
    })
}

fn view(state: &State) -> Element<Message> {
    container(
        column![
            row![
                "File: ",
                text(match state.file_path {
                    Some(ref path) => path.display().to_string(),
                    None => String::from(""),
                })
            ]
            .spacing(10),
            button("Open ...").on_press(Message::FileDialogRequest),
            row![
                "Mouse position: ",
                text(state.mouse_position.unwrap_or_default().to_string())
            ]
            .spacing(10),
        ]
        .spacing(10),
    )
    .padding(10)
    .into()
}

#[derive(Debug, Clone)]
enum Message {
    FileDialogRequest,
    FileDropped(PathBuf),
    MouseMoved(Point),
}

#[derive(Default)]
struct State {
    file_path: Option<PathBuf>,
    mouse_position: Option<Point>,
}

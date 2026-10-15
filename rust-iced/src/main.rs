use iced::widget::{button, column, container, row, scrollable, text};
use iced::{event, mouse, window, Border, Element, Event, Point, Theme};
use iced::{Size, Subscription, Task};
use rfd::AsyncFileDialog;
use std::path::PathBuf;

pub fn main() -> iced::Result {
    iced::application("Drag and drop", update, view)
        .window_size(Size::new(600.0, 200.0))
        .subscription(subscription)
        .run()
}

fn update(state: &mut State, message: Message) -> Task<Message> {
    match message {
        Message::FileDialogRequest => {
            return Task::perform(AsyncFileDialog::new().pick_files(), |handles| {
                let paths = handles
                    .unwrap_or_default()
                    .iter()
                    .map(|handle| handle.path().to_path_buf())
                    .collect();
                Message::FilesPicked(paths)
            });
        }
        Message::FilesPicked(paths) => {
            if !paths.is_empty() {
                state.file_paths = paths;
            }
        }
        Message::FileHovered => state.is_hovering = true,
        Message::FilesHoveredLeft => state.is_hovering = false,
        Message::FileDropped(path) => {
            // winit emits one event per file. The first one of a drop replaces the previous list.
            if state.is_hovering {
                state.is_hovering = false;
                state.file_paths.clear();
            }
            state.file_paths.push(path);
        }
        Message::MouseMoved(position) => state.mouse_position = Some(position),
    }
    Task::none()
}

fn subscription(_state: &State) -> Subscription<Message> {
    event::listen_with(|event, _status, _windows| match event {
        Event::Window(window::Event::FileHovered(_)) => Some(Message::FileHovered),
        Event::Window(window::Event::FilesHoveredLeft) => Some(Message::FilesHoveredLeft),
        Event::Window(window::Event::FileDropped(path)) => Some(Message::FileDropped(path)),
        Event::Mouse(mouse::Event::CursorMoved { position }) => Some(Message::MouseMoved(position)),
        _ => None,
    })
}

fn view(state: &State) -> Element<'_, Message> {
    let paths = state
        .file_paths
        .iter()
        .map(|path| path.display().to_string())
        .collect::<Vec<_>>()
        .join("\n");
    let is_hovering = state.is_hovering;

    container(
        column![
            // The list takes the remaining height and scrolls, so the controls stay visible
            row![
                "File: ",
                scrollable(text(paths)).width(iced::Fill).height(iced::Fill)
            ]
            .spacing(10)
            .height(iced::Fill),
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
    .width(iced::Fill)
    .height(iced::Fill)
    // The drop event has no position, so the whole window is the drop area
    .style(move |theme: &Theme| {
        let palette = theme.extended_palette();
        let color = if is_hovering {
            palette.primary.strong.color
        } else {
            palette.background.base.color
        };
        container::Style {
            border: Border {
                color,
                width: 4.0,
                ..Border::default()
            },
            ..container::Style::default()
        }
    })
    .into()
}

#[derive(Debug, Clone)]
enum Message {
    FileDialogRequest,
    FilesPicked(Vec<PathBuf>),
    FileHovered,
    FilesHoveredLeft,
    FileDropped(PathBuf),
    MouseMoved(Point),
}

#[derive(Default)]
struct State {
    file_paths: Vec<PathBuf>,
    is_hovering: bool,
    mouse_position: Option<Point>,
}

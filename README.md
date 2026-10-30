# 👻 Horror-Granny

A spine-chilling survival horror game built with **Unity**. You are trapped inside a creepy house and must explore your surroundings, collect keys, solve puzzles, and escape before Granny catches you.

Every sound matters. Move quietly, hide wisely, and stay alert — Granny can hear you!

---

## 📖 Table of Contents

- [About the Game](#about-the-game)
- [Features](#features)
- [Requirements](#requirements)
- [Getting Started](#getting-started)
- [How to Make the Game](#how-to-make-the-game)
- [Project Structure](#project-structure)
- [How to Play](#how-to-play)
- [Build the Game](#build-the-game)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)

---

## 🎮 About the Game

**Horror-Granny** is a first-person survival horror project where players must investigate a dangerous environment, discover useful items, solve puzzles, and find a way out without being caught.

The game focuses on stealth, exploration, suspense, and survival.

For Android, install **Android Build Support**, **Android SDK & NDK Tools**, and **OpenJDK** for Unity 2020.2.0f1. Build an APK from the Android target in **File > Build Settings**.

### Current UI and Controls

- Main menu settings include volume, mouse sensitivity, and fullscreen mode.
- In-game UI shows the picture objective, interaction prompts, pause/resume, and win/game-over screens.
- PC controls: WASD or arrow keys to move, mouse to look, Left Shift to run, C to crouch, Left Mouse Button to interact, and Esc to pause.
- Mobile controls are enabled automatically on supported devices.

## ✨ Features

- 👻 Horror-themed atmosphere
- 🏃 First-person exploration and movement
- 🔑 Collectible keys and quest items
- 🧩 Puzzle-solving gameplay
- 🤫 Stealth and hiding mechanics
- 🔊 Atmospheric sound effects
- 🚪 Escape objectives
- 🗺️ Multiple scenes and demo environments

## 🛠️ Requirements

Before starting, install the following tools:

- **Unity Hub** — to manage Unity projects and editor versions.
- **Unity 2020.2.0f1** — the project's original Unity editor version.
- **Git** — optional, for cloning and managing the repository.
- **Visual Studio or another supported code editor** — for editing C# scripts.
- A Windows, macOS, or Linux computer capable of running the Unity Editor.

> **Note:** Use Unity 2020.2.0f1 if you want to match the original project version. It is an old release, so if it is unavailable, a compatible Unity version may work, but project compatibility is not guaranteed.

## 🚀 Getting Started

### Step 1: Download the Project

Clone the repository using Git:

```bash
git clone <YOUR_REPOSITORY_URL>
```

Replace `<YOUR_REPOSITORY_URL>` with your actual Git repository URL.

Alternatively, download the repository as a ZIP file and extract it to your computer.

### Step 2: Install Unity Hub

1. Download Unity Hub from https://unity.com/download.
2. Install and launch Unity Hub.
3. Sign in or create a Unity account if required.

### Step 3: Install Unity 2020.2.0f1

1. Open Unity Hub.
2. Go to **Installs**.
3. Select **Install Editor** or the equivalent option.
4. Find Unity 2020.2.0f1 if it is available.
5. Install the modules needed for your target platform, such as Windows Build Support.

### Step 4: Open the Project

1. Open Unity Hub.
2. Select **Projects**.
3. Click **Add** or **Add project from disk**.
4. Select the root folder containing `Assets`, `Packages`, and `ProjectSettings`.
5. Open the project using the intended Unity version.
6. Wait for Unity to import the assets and prepare the project.

### Step 5: Open the Game Scene

In the Unity Editor, locate and open:

```text
Assets/HorrorEyes/HE Project/Scenes/GameStartScene.unity
```

Click the **Play** button at the top of the Unity Editor to test the game.

If the scene fails to load, check the Unity Console for missing assets, scripts, or package errors.

---

## 🧱 How to Make the Game

Use the following workflow if you want to learn how to create your own Granny-style survival horror game in Unity.

### Step 1: Create a Unity Project

1. Open Unity Hub.
2. Click **New Project**.
3. Select a suitable 3D template.
4. Name your project `Horror-Granny`.
5. Choose a folder and click **Create Project**.

If you are modifying this existing repository, skip this step and open the downloaded project instead.

### Step 2: Create the House Environment

Build the environment using 3D objects or suitable assets.

1. Create floors, walls, ceilings, doors, and windows.
2. Add rooms, corridors, stairs, and hiding places.
3. Use dim lighting and shadows to create suspense.
4. Add furniture and objects that can block movement.
5. Add colliders to walls and solid objects so the player cannot walk through them.

You can create basic geometry using Unity's built-in 3D objects or import assets you have permission to use.

### Step 3: Add the Player

1. Create a player GameObject.
2. Add a Character Controller or another suitable movement system.
3. Add a camera at the player's eye level.
4. Write or configure a C# script for walking, looking around, and interacting with objects.
5. Test the movement and collision behavior.

### Step 4: Add Granny

1. Import or create a suitable enemy character.
2. Add an Animator if the character has animations.
3. Configure colliders and navigation components.
4. Use a NavMesh and an AI script to let Granny patrol and chase the player.
5. Add detection logic so the enemy can react to player visibility, distance, or noise.

### Step 5: Add Keys and Puzzles

1. Create key objects and place them throughout the house.
2. Add interaction logic for collecting keys.
3. Create locked doors that require specific items.
4. Add puzzles such as switches, codes, or hidden objects.
5. Update the player's objectives when items are collected or puzzles are solved.

### Step 6: Add Sound and Horror Effects

1. Add background ambience and environmental sounds.
2. Use Audio Sources for footsteps, doors, and object interactions.
3. Trigger sounds when the player makes noise.
4. Add suitable lighting, fog, and visual effects.
5. Balance the volume so important sound cues remain noticeable.

### Step 7: Create the Escape System

1. Design one or more escape objectives.
2. Check whether the player has collected the required items.
3. Unlock the exit when the objectives are complete.
4. Trigger a win screen or ending scene when the player escapes.
5. Add a game-over screen if Granny catches the player.

### Step 8: Test and Improve

Test the game regularly in the Unity Editor.

- Check player movement and collisions.
- Test keys, doors, and puzzles.
- Verify Granny's patrol and chase behavior.
- Check that audio and scene transitions work.
- Fix errors shown in the Unity Console.
- Optimize lighting, models, and performance.

**Important:** These steps describe a development workflow. They do not mean all the listed mechanics are already implemented in this repository.

---

## 📂 Project Structure

The project includes the following known scene path and build scenes:

```text
Horror-Granny/
├── Assets/
│   └── HorrorEyes/
│       └── HE Project/
│           └── Scenes/
│               └── GameStartScene.unity
├── Packages/
├── ProjectSettings/
└── README.md
```

The full contents may vary depending on the repository version. Keep the `Assets`, `Packages`, and `ProjectSettings` folders together when sharing or moving the Unity project.

## 🕹️ How to Play

The exact controls depend on the scripts configured in the project. If the default controls are not documented in-game, inspect the player controller and input settings.

Typical survival horror objectives include:

1. Explore the house carefully.
2. Search rooms and containers for useful items.
3. Collect keys and solve puzzles.
4. Avoid making unnecessary noise.
5. Hide when the enemy is nearby.
6. Complete the escape objectives and reach the exit.

## 📦 Build the Game

To create a playable build:

1. Open the project in Unity.
2. Select **File → Build Settings**.
3. Choose the target platform, such as Windows.
4. Add the required scenes to the build list.
5. Verify the scene order and platform settings.
6. Click **Build**.
7. Select an output folder and wait for Unity to finish.

The project's listed build scenes are:

- `GameStartScene`
- `MainMenu`
- `LoadScene`
- `Morgue`
- `School_Demo`

Check the actual scenes available in the project before building. Include only the scenes required for your intended game flow.

## 🐛 Troubleshooting

**Unity cannot open the project**

- Confirm that you selected the project root folder.
- Check the Unity Editor version.
- Allow Unity to finish importing assets.

**Scripts show errors**

- Open **Window → General → Console**.
- Read the first error and resolve missing scripts, dependencies, or incompatible APIs.
- Recheck the Console after fixing the error.

**A scene is missing**

- Search the `Assets` directory for the scene file.
- Confirm that the repository download is complete.
- Add the scene to Build Settings if it needs to be included in the build.

**The game runs but the player cannot move**

- Check the player controller component.
- Verify that the required input system is configured.
- Check the Console for script or input errors.

**The build fails**

- Confirm that the correct platform module is installed.
- Check that all required scenes and assets are included.
- Review the Console and build log for the underlying error.

## 🤝 Contributing

Contributions, suggestions, and bug reports are welcome.

1. Fork the repository.
2. Create a feature branch.
3. Make and test your changes.
4. Commit your changes with a clear message.
5. Submit a pull request describing your improvements.

Please respect the licenses of any third-party models, textures, audio, and other assets you use.

## 📜 License

Add the appropriate license information for this project and its assets before redistributing the game.

## ❤️ Thank You

Thank you for checking out **Horror-Granny**!

Explore carefully. Stay quiet. Stay alive.

**The Horror-Granny Team**

# Player Controller và Core Foundation

Tài liệu này ghi lại hai nhóm hệ thống đã được xây trong prototype Unity: hệ thống điều khiển người chơi 3D và nền core network/level. Mục tiêu là giúp các bước phát triển tiếp theo có điểm tựa rõ ràng về kiến trúc, luồng dữ liệu, public API và trạng thái kiểm thử hiện tại.

## 1. Hệ Thống Điều Khiển Người Chơi

Hệ thống player controller hiện được đặt trong namespace `NetworkingSimulation.Player` và tách thành các component nhỏ để dễ chạy local trước, đồng thời thuận lợi khi bọc multiplayer authority sau này. Input chỉ được đọc tại `PlayerInputReader`; movement, camera và interaction nhận dữ liệu qua reference/event thay vì tự gọi trực tiếp Unity Input.

### Thành phần chính

- `PlayerInputReader`: đọc action map `Player` từ `InputSystem_Actions.inputactions`. Component này expose input dạng state qua `Move`, `Look`, `IsSprinting` và expose input dạng sự kiện qua `JumpPressed`, `InteractPressed`, `PerspectiveTogglePressed`.
- `PlayerMotor`: dùng `CharacterController` để xử lý di chuyển ngang, sprint, jump, gravity và grounded state. Component này đọc input thông qua `PlayerInputReader`, lấy hướng ngang từ camera, và xoay thân player theo mode camera hiện tại.
- `PlayerCameraRig`: quản lý một camera chính duy nhất với hai mode `ThirdPerson` và `FirstPerson`. Rig dùng chung `yaw/pitch` để chuyển góc nhìn bằng Tab mà không bị lệch hướng.
- `PlayerInteractionRaycaster`: raycast từ camera hiện tại về phía trước, tìm object implement `IPlayerInteractable`, lưu target/prompt hiện tại và gọi `Interact` khi input interact được nhấn.
- `IPlayerInteractable`: interface gameplay tối thiểu cho các object có thể tương tác về sau như thiết bị mạng, port, bảng điều khiển, vùng spawn hoặc node physical.

### Input hiện có

Action map `Player` đang dùng Input System sẵn có của project:

- `Move`: WASD, arrow keys, gamepad left stick và các binding mặc định khác.
- `Look`: mouse delta, gamepad right stick và binding mặc định khác.
- `Sprint`: `Left Shift` trên keyboard, gamepad left stick press.
- `Jump`: `Space` trên keyboard, gamepad south button.
- `Interact`: `E` trên keyboard, gamepad north button.
- `TogglePerspective`: `Tab` trên keyboard.

Các action name được serialize trong `PlayerInputReader`, nên nếu đổi tên action trong asset input thì cần cập nhật inspector hoặc default field tương ứng.

### Movement

`PlayerMotor` yêu cầu `CharacterController` trên player root. Mỗi frame, motor lấy `Move` từ input reader, clamp magnitude về tối đa `1`, rồi chiếu hướng `cameraTransform.forward/right` lên mặt phẳng ngang để tạo movement relative theo camera.

Tốc độ mặc định:

- Walk speed: `4.5`
- Sprint speed: `7.0`
- Rotation speed: `12`
- Jump height: `1.4`
- Gravity: `-20`
- Grounded stick force: `-2`

Jump được xử lý bằng event `JumpPressed`. Khi jump được request và `CharacterController.isGrounded` là true, motor set vertical velocity theo công thức `sqrt(jumpHeight * -2 * gravity)`. Nếu đang grounded và vertical velocity âm, motor giữ player bám đất bằng `groundedStickForce`.

Trong third-person, thân player xoay mượt về hướng di chuyển. Trong first-person, thân player đồng bộ ngay theo `cameraRig.Yaw`, kể cả khi đứng yên, để WASD luôn đi theo hướng nhìn ngang.

### Camera và chuyển góc nhìn

`PlayerCameraRig` expose public API:

- `PlayerCameraPerspective CurrentPerspective`
- `event Action<PlayerCameraPerspective> PerspectiveChanged`
- `void SetPerspective(PlayerCameraPerspective perspective)`
- `void TogglePerspective()`
- `float Yaw`

Mode `ThirdPerson` giữ camera orbit/follow quanh `thirdPersonTarget`. Rig tính focus point bằng vị trí target cộng `targetHeight`, rồi đặt camera ở sau focus theo `distance`. Camera collision dùng `Physics.SphereCast` với `collisionRadius`, `minimumDistance`, `collisionMask` để tránh xuyên tường hoặc ground.

Mode `FirstPerson` đặt camera tại `firstPersonAnchor`; nếu anchor không có thì fallback về `playerRoot.position + Vector3.up * fallbackEyeHeight`. Khi vào first-person, rig xoay `playerRoot` theo yaw, đặt camera rotation theo `pitch/yaw`, và ẩn `playerVisual`. Khi quay lại third-person, visual được bật lại.

Thông số mặc định quan trọng:

- Third-person distance: `5`
- Target height: `1.6`
- Fallback eye height: `1.65`
- Sensitivity: `0.12`
- Pitch clamp: `-80` đến `80`
- Collision radius: `0.25`
- Minimum camera distance: `0.75`

### Interaction raycast

`PlayerInteractionRaycaster` dùng `cameraTransform.position` và `cameraTransform.forward`, nên tương tác tự hoạt động đúng ở cả third-person và first-person. Mỗi frame component refresh target hiện tại:

1. Tạo ray từ camera.
2. Raycast trong `interactionRange` với `interactionMask` và `triggerInteraction`.
3. Tìm `IPlayerInteractable` trên collider hoặc parent.
4. Tạo `PlayerInteractorContext`.
5. Chỉ giữ target nếu `CanInteract(context)` trả true.

Khi người chơi nhấn `Interact`, component refresh target lần nữa rồi gọi `currentInteractable.Interact(BuildContext())`.

`PlayerInteractorContext` chứa:

- `GameObject Player`
- `Transform CameraTransform`
- `RaycastHit Hit`
- `PlayerInputReader Input`

V1 chưa có UI prompt/crosshair; `CurrentPrompt` đã có sẵn để UI sau này đọc từ `InteractionPrompt`.

### Scene setup hiện tại

Scene mẫu có player root với `CharacterController`, các component input/motor/interaction, visual capsule placeholder, `Camera Target` cho third-person và `Eye Anchor` cho first-person. Camera chính gắn `PlayerCameraRig` và reference về player/input/target/anchor/visual.

Thiết kế này cố tình chưa phụ thuộc Cinemachine, Photon hoặc Fusion. Khi tích hợp multiplayer, hướng đi dự kiến là thay nguồn input/authority ở lớp input hoặc adapter bên ngoài, giữ `PlayerMotor` và core camera behavior ít thay đổi nhất có thể.

## 2. Core Network + Level Foundation

Core foundation hiện nằm trong namespace `NetworkingSimulation.Core`, `NetworkingSimulation.Core.Network` và `NetworkingSimulation.Core.Levels`. Mục tiêu của V1 là có nền topology, validation, level data và economy có thể test độc lập trước khi làm visual placement, cable dragging hoặc routing simulation thật.

### Network graph

`NetworkGraph` quản lý node và edge bằng:

- `IReadOnlyCollection<NetworkNode> Nodes`
- `IReadOnlyCollection<NetworkEdge> Edges`
- `bool AddNode(NetworkNode node)`
- `bool RemoveNode(string nodeId)`
- `void Clear()`
- `bool TryGetNode(string nodeId, out NetworkNode node)`
- `ConnectResult ConnectNodes(string nodeAId, int portAId, string nodeBId, int portBId, float length)`
- `DisconnectResult DisconnectPort(string nodeId, int portId)`
- `RouteSimulationResult SimulateRouting(Packet packet)`

Graph lưu node bằng `NodeId` và lưu edge hai chiều trong list. Khi connect thành công, graph tạo một `NetworkEdge`, thêm vào danh sách edge, rồi bind cả hai `NetworkPort`. Khi disconnect hoặc remove node, graph remove edge liên quan và release cả hai port.

### Node, port và edge

`NetworkNode` là abstract base chứa:

- `string NodeId`
- `string DisplayName`
- `string IPAddress`
- `NetworkNodeType NodeType`
- `IReadOnlyList<NetworkPort> LogicPorts`
- `bool TryGetPort(int portId, out NetworkPort port)`

Các node V1:

- `RouterNode`: mặc định 4 port.
- `SwitchNode`: mặc định 8 port.
- `EndDeviceNode`: mặc định 1 port.

`NetworkPort` chứa trạng thái logic của một port:

- `int PortId`
- `bool IsOccupied`
- `string ConnectedNodeId`
- `int? ConnectedPortId`
- `Bind(string nodeId, int portId)`
- `Release()`

`NetworkEdge` đại diện kết nối hai chiều giữa hai port, kèm `Length`.

### Connect/disconnect validation

`ConnectNodes` trả về `ConnectResult` thay vì throw cho các lỗi topology thông thường:

- `Success`
- `MissingNode`
- `InvalidPort`
- `PortOccupied`
- `SelfConnection`
- `InvalidLength`
- `AlreadyConnected`

Thứ tự validation hiện tại: length âm, self connection, missing node, invalid port, already connected, occupied port. Kết nối thành công sẽ occupy cả hai port.

`DisconnectPort` trả về `DisconnectResult`:

- `Success`
- `MissingNode`
- `InvalidPort`
- `NoConnection`

Disconnect thành công luôn giải phóng cả hai đầu kết nối.

### Economy

`EconomyManager` là class logic thuần, không phải `MonoBehaviour`. API hiện tại:

- `float CurrentBudget`
- `void SetBudget(float amount)`
- `bool DeductBudget(float amount)`
- `void RefundBudget(float amount)`

Các amount âm bị từ chối bằng `ArgumentOutOfRangeException`. `DeductBudget` trả false nếu không đủ ngân sách, không làm budget âm.

### Level data và level lifecycle

`LevelData` và các DTO phụ được thiết kế để thân thiện với `JsonUtility`:

- `levelId`
- `title`
- `description`
- `objectiveDescription`
- `sourceVillageIP`
- `destinationVillageIP`
- `startingBudget`
- `timeLimitInSeconds`
- `fixedNodes`
- `providedTools`

`FixedNodeData` chứa `id`, `name`, `type`, `ipAddress`, `position`. `ProvidedToolData` chứa `type`, `maxQuantity`. `SerializableVector3Data` có `x/y/z` và `ToVector3()`.

`LevelManager` cũng là class logic thuần. API chính:

- `LevelData CurrentLevel`
- `float TimeRemaining`
- `bool IsLevelActive`
- `NetworkGraph NetworkGraph`
- `EconomyManager EconomyManager`
- `void LoadLevel(LevelData data)`
- `void StartLevel()`
- `void UpdateTimer(float deltaTime)`
- Events: `LevelLoaded`, `LevelStarted`, `LevelFailed`

`LoadLevel` set current level, reset timer, reset active state, set budget, clear graph và tạo fixed nodes vào graph. Node type được parse từ string, nếu type không hợp lệ thì fallback về `EndDevice`.

`StartLevel` yêu cầu đã load level, sau đó bật `IsLevelActive` và fire `LevelStarted`. `UpdateTimer` chỉ chạy khi level active và deltaTime dương; khi timer về 0, level dừng và fire `LevelFailed`.

### Routing stub

`NetworkGraph.SimulateRouting(Packet packet)` hiện trả `RouteSimulationResult.NotImplemented(packet)`. Đây là stub có chủ ý để giữ public surface cho routing về sau, nhưng V1 chưa triển khai thuật toán router/switch, ARP, subnet, packet traversal hoặc scoring.

## 3. Trạng Thái Kiểm Thử

Các lỗi compile trong Unity trước đó đến từ test assembly không reference được runtime code. Cách sửa đã thực hiện:

- Thêm `NetworkingSimulation.Runtime.asmdef` dưới `Assets/Scripts`.
- Đổi `NetworkingSimulation.EditModeTests.asmdef` để reference `NetworkingSimulation.Runtime` thay vì `Assembly-CSharp`.
- Refresh AssetDatabase để Unity sinh `NetworkingSimulation.Runtime.csproj` và cập nhật `NetworkingSimulation.EditModeTests.csproj`.

Kết quả xác nhận gần nhất:

- Unity recompile: `0 warnings`, `0 errors`.
- `dotnet build Assembly-CSharp.csproj`: sạch.
- `dotnet build Assembly-CSharp-Editor.csproj`: sạch.
- `dotnet build NetworkingSimulation.EditModeTests.csproj`: sạch.
- Unity EditMode tests: `13/13 passed`, `0 failed`, `0 skipped`.
- Unity Console error logs qua MCP: `[]`.

Các test hiện bao phủ:

- Add node và từ chối duplicate `NodeId`.
- Connect hợp lệ tạo một edge hai chiều và occupy cả hai port.
- Connect fail với missing node, invalid port, self connection, invalid length, occupied port.
- Disconnect giải phóng cả hai port và remove edge.
- Disconnect fail với missing node, invalid port, no connection.
- Remove node remove edge liên quan và release port còn lại.
- Economy deduct/refund, insufficient funds và amount âm.
- LevelManager load level, deserialize JSON bằng `JsonUtility`, reset graph, start timer và fail khi hết giờ.

## 4. Việc Tiếp Theo Đề Xuất

### Player controller

- Thêm cursor lock/unlock và mouse capture policy rõ ràng cho Play Mode.
- Thêm crosshair và interaction prompt UI đọc từ `PlayerInteractionRaycaster.CurrentPrompt`.
- Tạo object test implement `IPlayerInteractable` trong scene để kiểm tra interaction bằng `E`.
- Thêm settings sensitivity/invert Y nếu cần.
- Polish camera: smooth follow, optional shoulder offset, mode transition blend, và collision mask riêng thay vì `~0`.
- Chuẩn bị multiplayer adapter: tách input snapshot/local authority để sau này đưa vào Photon Fusion mà không sửa motor quá nhiều.

### Core foundation

- Triển khai routing simulation thật trên `NetworkGraph`, bắt đầu từ pathfinding đơn giản rồi mở rộng sang rule router/switch.
- Tạo bridge giữa logic node/port và visual object: `NodePhysical`, `PortPhysical`, cable object.
- Tích hợp grid placement/snapping để spawned devices có thể tạo/remove node trong graph.
- Tạo level JSON loader hoặc ScriptableObject importer để load `LevelData` từ file thay vì tạo trực tiếp trong test.
- Thêm validation level data: duplicate fixed node id, missing IP, invalid node type, budget/time âm.
- Kết nối Economy với placement/cable cost để `DeductBudget` và `RefundBudget` được dùng trong gameplay loop thật.

## 5. Giới Hạn Hiện Tại

- Chưa có gameplay visual cho thiết bị mạng, port hoặc dây/cáp.
- Chưa có routing packet thật.
- Chưa có UI objective, timer, budget hoặc prompt.
- Chưa có multiplayer runtime/authority.
- Player visual hiện chỉ là capsule placeholder, nên first-person đang ẩn toàn bộ visual local.

# 라이브 생존 방송

인터넷방송 컨셉의 Unity 2D 생존 액션 프로토타입입니다. 스트리머가 방송 중 몰려오는 악성 채팅과 알고리즘 압박을 버티며 관심도를 모으고 방송을 성장시키는 방향으로 잡았습니다.

## 실행

1. Unity Hub에서 이 폴더를 엽니다.
2. 패키지 복원이나 입력 처리 변경으로 재시작을 요구하면 Unity를 재시작합니다.
3. Unity가 모바일 플랫폼으로 열리면 `Tools > 라이브 생존 방송 > PC 화면 설정`을 실행합니다.
4. `Assets/Scenes/00_Boot.unity`를 열거나 Build Settings의 첫 씬으로 실행합니다.
5. Play를 누릅니다.

게임은 런타임에 자동으로 구성되므로 프리팹이나 씬 배치가 필요 없습니다.

Unity 6.3 기준으로 Input System 패키지를 사용하며, Active Input Handling은 Input System Package로 설정되어 있습니다.

## 조작

- 이동: WASD 또는 방향키
- 강화 선택: 마우스 클릭
- 일시정지: ESC
- 방송 종료 후 재시작: R

## 메뉴

- 시작 화면에서 `방송 시작`, `조작 안내`, `게임 종료`를 선택할 수 있습니다.
- 플레이 중 ESC를 누르면 `계속 방송`, `처음부터 다시`, `메인 메뉴`를 선택할 수 있습니다.
- 방송 종료 후에는 `다시 방송하기` 또는 `메인 메뉴`로 돌아갈 수 있습니다.

## 프로젝트 구조

- `Assets/Scenes`: 실행 흐름용 씬입니다. `00_Boot`, `01_MainMenu`, `02_Arena`로 나눴습니다.
- `Assets/Scripts/Core`: 게임 부트스트랩, 전체 게임 흐름, HUD와 메뉴를 관리합니다.
- `Assets/Scripts/Actors`: 플레이어와 적 캐릭터 동작을 관리합니다.
- `Assets/Scripts/Combat`: 자동 공격과 투사체를 관리합니다.
- `Assets/Scripts/Pickups`: 관심도 획득 아이템을 관리합니다.
- `Assets/Scripts/Rendering`: 런타임 스프라이트, 타일, 카메라, 정렬 보조 스크립트를 관리합니다.
- `Assets/Scripts/Data`: 강화 타입과 선택지 같은 데이터 구조를 둡니다.
- `Assets/Art`, `Assets/Prefabs`, `Assets/Audio`, `Assets/Materials`, `Assets/Settings`: 이후 실제 리소스가 들어갈 자리입니다.

## 아트 리소스

- `Assets/Art/External/OpenGameArt/TopDownShooterAssetPack`: OpenGameArt의 CC0 탑다운 슈터 에셋 원본을 보관합니다.
- `Assets/Resources/GameArt`: 런타임에서 우선 로드하는 선별 PNG입니다. 없으면 기존 런타임 생성 스프라이트로 자동 대체됩니다.
- 현재 적용된 외부 리소스 출처: https://opengameart.org/content/top-down-shooter-asset-pack

## 현재 루프

- 배경은 방송 스튜디오/채팅창 느낌의 `Grid`와 2D `Tilemap`으로 런타임 생성됩니다.
- 스트리머, 악성 채팅, 알고리즘 봇, 관심도 하트, 밴 해머는 런타임 픽셀아트 스프라이트로 생성됩니다.
- 필드는 두꺼운 선이 있는 탑다운 아레나 타일로 구성되고, 캐릭터/적/아이템은 굵은 외곽선과 약한 바닥 그림자로 표현됩니다.
- 플레이어는 스트리머 역할로 탑다운 아레나에서 이동합니다.
- 악성 채팅이 카메라 주변에서 몰려와 플레이어를 압박합니다.
- 플레이어는 가장 가까운 적을 자동으로 모더레이션합니다.
- 악성 채팅은 정리되면 관심도를 떨어뜨립니다.
- 팔로워 레벨이 오르면 방송 성장 강화 중 하나를 선택합니다.
- 시간이 지날수록 채팅 압박이 강해집니다.

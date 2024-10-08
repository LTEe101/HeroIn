using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UI_Game_Score : UI_Scene
{
    enum Texts
    {
        ScoreText,
        TotalText,
    }

    int score = 0;
    private float holdTime = 0f; // 클릭 시간
    private bool isHolding = false; // 꾹 누르고 있는지 여부
    private GameObject target;
    private GameObject particleInstance;
    private GameObject boom;

    private GameObject[] cannonBalls;

    private Animator[] anims;
    public System.Action onFinished;

    // 게임 모드 구분 변수 (게임 2일 경우 true로 설정)
    private bool isGameTwo = false;
    private EnemyManager enemyManager; // 게임 2에서 적이 제거될 때 점수를 추가하기 위해 필요
    private Slider _totalBar;
    public void SetGameMode(bool gameTwo)
    {
        isGameTwo = gameTwo;
    }

    void Start()
    {
        Init();

        if (isGameTwo)
        {
            // 게임 2에서는 EnemyManager와 상호작용
            enemyManager = EnemyManager.Instance;
            if (enemyManager != null)
            {
                enemyManager.onEnemyDestroyed += OnEnemyDestroyed; // 적 제거 시 점수 증가
            }
        }
        else
        {
            // 게임 1의 로직 (클릭 이벤트 처리)
            Managers.Input.MouseAction -= OnMouseClicked;
            Managers.Input.MouseAction += OnMouseClicked;
            boom = Managers.Resource.Instantiate($"CFXR Explosion 1");
            boom.SetActive(false);

            cannonBalls = GameObject.FindGameObjectsWithTag("CannonBall");
            anims = new Animator[3];
            for (int i = 0; i < 3; i++)
            {
                if (cannonBalls[i] != null)
                {
                    anims[i] = cannonBalls[i].GetComponent<Animator>();
                }
            }
        }
    }

    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
    }

    // 게임 2에서 적이 제거될 때 점수 추가
    private void OnEnemyDestroyed()
    {
        score++;
        UpdateScoreText();
        CheckGameEnd(); // 스코어가 3일 때 성공 팝업 띄우기
    }

    // 게임 1의 기존 클릭 및 애니메이션 처리 로직
    private GameObject _gameBarBox;
    private bool hasScored = false;
    private AudioSource _effectSource;
    private void Update()
    {
        if (!isGameTwo && isHolding)
        {

            holdTime += Time.deltaTime;

            if (_effectSource == null)
            {
                _effectSource = Managers.Sound.PlayReturnAudioSource("2000Effect/MECHS - MACHINES - SERVOS - (103)/MECH Servo Motor Power Up Long 03", Define.Sound.Effect, 0.4f);
            }

            if (_gameBarBox == null && target != null)
            {
                _gameBarBox = Managers.Resource.Instantiate("GameBarBox");
                _gameBarBox.transform.SetParent(target.transform, false); // 부모로 설정

                if (_gameBarBox != null)
                {
                    Transform uiGameBar = _gameBarBox.transform.GetChild(0); // 첫 번째 자식 (UI_Game_Bar)
                    if (uiGameBar != null)
                    {
                        _totalBar = uiGameBar.GetChild(0).GetComponent<Slider>(); // 첫 번째 자식 (TotalBar)에서 Slider 가져옴
                    }
                }
            }

            if (_totalBar != null)
            {
                _totalBar.value = Mathf.Clamp01(holdTime / 2f); // 2초 동안 슬라이더가 다 차도록 설정
            }

            if (holdTime >= 2f && target != null && particleInstance == null && !hasScored)
            {
                hasScored = true;

                // 포탄 날라가는 효과
                switch (target.name)
                {
                    case "TargetShip3":
                        if (anims[0] != null)
                        {
                            StartCoroutine(PlayAnimationAndSpawnParticle(anims[0], target));
                        }
                        break;
                    case "TargetShip1":
                        if (anims[1] != null)
                        {
                            StartCoroutine(PlayAnimationAndSpawnParticle(anims[1], target));
                        }
                        break;
                    case "TargetShip2":
                        if (anims[2] != null)
                        {
                            StartCoroutine(PlayAnimationAndSpawnParticle(anims[2], target));
                        }
                        break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetHold();
        }
    }

    private IEnumerator PlayAnimationAndSpawnParticle(Animator animator, GameObject target)
    {
        GameObject ball = animator.gameObject;
        animator.SetTrigger("ShootTrigger");

        // 애니메이션 길이 가져오기
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength + 0.4f);

        if (particleInstance == null)
        {
            // 폭발 효과 생성
            Vector3 particlePosition = target.transform.position + new Vector3(0, 10, 5);
            particleInstance = Instantiate(boom, particlePosition, target.transform.rotation);
            particleInstance.SetActive(true);

            // 게임 1 효과음 재생
            AudioClip audioClip = Managers.Sound.GetOrAddAudioClip("ProEffect/Explosion_Fire_Gas/explosion_large_no_tail_02", Define.Sound.Effect);
            Managers.Sound.Play(audioClip, Define.Sound.Effect, 0.5f);
            Destroy(ball);
            Destroy(target);
            if (audioClip != null)
            {
                yield return new WaitForSeconds(audioClip.length); // 효과음 길이만큼 대기
            }
        }

        // 득점
        score++;
        UpdateScoreText();

        if (_gameBarBox != null)
        {
            Destroy(_gameBarBox);
            _gameBarBox = null;
        }

        // 배 없애기
        
        isHolding = false;
        hasScored = false;

        CheckGameEnd(); // 스코어가 3일 때 성공 팝업 띄우기
    }

    private void CheckGameEnd()
    {
        if (score >= 3) // 스코어가 3 이상이면 성공 팝업
        {
            // Close the UI_Motion_State before showing the finish UI
            var motionStateUI = FindObjectOfType<UI_Motion_State>();
            if (motionStateUI != null)
            {
                motionStateUI.Close(); // Close the UI_Motion_State
            }

            TriggerEnemyDie();

            Managers.Sound.Play("ProEffect/Collectibles_Items_Powerup/points_ticker_bonus_score_reward_jingle_02", Define.Sound.Effect);

            Managers.UI.ShowPopupUI<UI_Game_Finish>(); // 성공 팝업 표시
            StartCoroutine(NextScene(5f)); // 5초 후에 다음 씬으로 전환
        }
    }

    private void TriggerEnemyDie()
    {
        // 'Enemy' 태그로 모든 적 오브젝트 찾기
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 각 적의 Animator에서 Die 트리거 활성화
        foreach (GameObject enemy in enemies)
        {
            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Die"); // Die 트리거 활성화
            }
        }
    }


    private IEnumerator NextScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // 대기

        // 다음 씬으로 전환
        if (!isGameTwo)
        {
            Managers.Scene.LoadScene(Define.Scene.StoryFour);
        }
        else
        {
            Managers.Scene.LoadScene(Define.Scene.StorySix);
        }
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (!isGameTwo && evt == Define.MouseEvent.Press) // 게임 1일 때만 동작
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int mask = (1 << 7); // 레이어 마스크 (7번 레이어가 Target임을 가정)
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f, mask))
            {
                target = hit.collider.gameObject; // 클릭한 오브젝트 저장
                isHolding = true; // 꾹 누르기 시작
            }
        }
        if (evt == Define.MouseEvent.PointerEnter)
        {
            // Outline 컴포넌트가 있는지 확인
            Outline outline = Managers.Input.HoveredObject.GetComponent<Outline>();
            if (outline != null)
            {
                // 아웃라인 모드를 OutlineAll 설정
                outline.OutlineMode = Outline.Mode.OutlineAll;
            }
        }
        if (evt == Define.MouseEvent.PointerExit)
        {
            // Outline 컴포넌트가 있는지 확인
            Outline outline = Managers.Input.HoveredObject.GetComponent<Outline>();
            if (outline != null)
            {
                // 아웃라인 모드를 OutlineHidden로 설정
                outline.OutlineMode = Outline.Mode.OutlineHidden;
            }
        }

    }

    private void ResetHold()
    {
        holdTime = 0f;
        target = null; // 클릭한 오브젝트 초기화
        isHolding = false;
        if (_gameBarBox != null)
        {
            Destroy(_gameBarBox); // GameBarBox 인스턴스 삭제
            _gameBarBox = null;
        }
        if (_effectSource != null)
        {
            _effectSource.Stop(); // 사운드 멈춤
            _effectSource = null;
        }
    }

    private void UpdateScoreText()
    {
        Get<Text>((int)Texts.ScoreText).GetComponent<Text>().text = score.ToString();
    }
}

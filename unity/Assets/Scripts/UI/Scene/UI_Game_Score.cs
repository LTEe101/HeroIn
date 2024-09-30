using System.Collections;
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
    private float holdTime = 0f;
    private bool isHolding = false;
    private GameObject target;
    private GameObject particleInstance;
    private GameObject boom;

    private GameObject[] cannonBalls;

    private Animator[] anims;
    public System.Action onFinished;

    private bool isGameTwo = false;
    private EnemyManager enemyManager;

    public void SetGameMode(bool gameTwo)
    {
        isGameTwo = gameTwo;
    }

    void Start()
    {
        Init();

        if (isGameTwo)
        {
            enemyManager = EnemyManager.Instance;
            if (enemyManager != null)
            {
                enemyManager.onEnemyDestroyed += OnEnemyDestroyed;
            }
        }
        else
        {
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

    private void OnEnemyDestroyed()
    {
        score++;
        UpdateScoreText();
        StartCoroutine(CheckGameEnd()); // 코루틴으로 호출
    }

    private UI_Game_Bar _bar = null;
    private bool hasScored = false;

    private void Update()
    {
        if (!isGameTwo && isHolding)
        {
            holdTime += Time.deltaTime;
            if (_bar == null && target != null)
            {
                _bar = Managers.UI.ShowPopupUI<UI_Game_Bar>();
                _bar.SetBarImagePosition(target.name);
            }

            float fillAmount = holdTime / 2f;
            _bar.SetFillAmount(fillAmount);

            // 게이지가 다 찼을 때
            if (holdTime >= 2f && target != null && particleInstance == null && !hasScored)
            {
                hasScored = true;

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

        // 마우스를 뗐을 때 게이지 UI를 닫음
        if (Input.GetMouseButtonUp(0))
        {
            ResetHold();
        }
    }

    private IEnumerator PlayAnimationAndSpawnParticle(Animator animator, GameObject target)
    {
        GameObject ball = animator.gameObject;
        animator.SetTrigger("ShootTrigger");

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength + 0.4f);

        if (particleInstance == null)
        {
            particleInstance = Instantiate(boom, target.transform.position, target.transform.rotation);
            particleInstance.SetActive(true);
            Managers.Sound.Play("ProEffect/Explosion_Fire_Gas/explosion_large_no_tail_02", Define.Sound.Effect, 0.2f);
        }

        Destroy(ball);

        score++;
        UpdateScoreText();

        Destroy(target); // 타겟 삭제
        isHolding = false;
        hasScored = false;

        // 애니메이션이 끝났을 때 게이지 닫기
        if (_bar != null)
        {
            _bar.ClosePopupUI();
            _bar = null;
        }

        StartCoroutine(CheckGameEnd()); // 코루틴으로 호출
    }

    private IEnumerator CheckGameEnd()
    {
        if (score >= 3)
        {
            yield return new WaitForSeconds(2f); // 2초 대기
            Managers.UI.ShowPopupUI<UI_Game_Finish>(); // 성공 팝업 표시
            Managers.Sound.Play("ProEffect/Collectibles_Items_Powerup/points_ticker_bonus_score_reward_jingle_03", Define.Sound.Effect, 1.2f); // 성공 소리 재생
            StartCoroutine(NextScene(4f)); // 4초 후에 다음 씬으로 전환
        }
    }

    private IEnumerator NextScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (isGameTwo)
        {
            //Managers.Scene.LoadScene(Define.Scene.StoryFour);
        }
        else
        {
            Managers.Scene.LoadScene(Define.Scene.StoryFour);
        }
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (!isGameTwo && evt == Define.MouseEvent.Press)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int mask = (1 << 7);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f, mask))
            {
                target = hit.collider.gameObject;
                isHolding = true;
            }
        }
    }

    private void ResetHold()
    {
        holdTime = 0f;
        target = null;
        isHolding = false;

        // 클릭이 중단되면 게이지 닫기
        if (_bar != null)
        {
            _bar.ClosePopupUI();
            _bar = null;
        }
    }

    private void UpdateScoreText()
    {
        Get<Text>((int)Texts.ScoreText).text = score.ToString();
    }
}

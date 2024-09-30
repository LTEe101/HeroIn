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
        CheckGameEnd();
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

        if (_bar != null)
        {
            _bar.ClosePopupUI();
            _bar = null;
        }

        Destroy(target);
        isHolding = false;
        hasScored = false;

        CheckGameEnd();
    }

    private IEnumerator CheckGameEnd()
{
    if (score >= 3)
    {
        yield return new WaitForSeconds(2f);
        Managers.UI.ShowPopupUI<UI_Game_Finish>();
        Managers.Sound.Play("ProEffect/Collectibles_Items_Powerup/points_ticker_bonus_score_reward_jingle_03", Define.Sound.Effect, 1.2f);
        StartCoroutine(NextScene(4f));
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

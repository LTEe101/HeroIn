using System.Collections;
using System.Collections.Generic;
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
    void Start()
    {
        Init();
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
    public override void Init()
    {
        base.Init();

        Bind<Text>(typeof(Texts));
    }
    private UI_Game_Bar _bar = null;
    private bool hasScored = false;
    private void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
            if (_bar == null && target != null)
            {
                _bar = Managers.UI.ShowPopupUI<UI_Game_Bar>();

                _bar.SetBarImagePosition(target.name); // 설정한 위치로 BarImage 위치 설정
            }

            float fillAmount = holdTime / 2f;
            _bar.SetFillAmount(fillAmount);

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
        {   // 누르다 뗐을 시 초기화
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
            particleInstance = Instantiate(boom, target.transform.position, target.transform.rotation);
            particleInstance.SetActive(true);
            Managers.Sound.Play("ProEffect/Explosion_Fire_Gas/explosion_large_no_tail_02", Define.Sound.Effect, 0.2f);
        }

        Destroy(ball);
        
        // 득점
        score++;
        UpdateScoreText();

        if (_bar != null)
        {
        // 게이지 UI 없애기
        _bar.ClosePopupUI();
        _bar = null;
        }

        // 배 없애기
        Destroy(target);
        isHolding = false;
        hasScored = false;

        if (score == 3)
        {
            yield return new WaitForSeconds(2f);
            Managers.UI.ShowPopupUI<UI_Game_Finish>();
            Managers.Sound.Play("ProEffect/Collectibles_Items_Powerup/points_ticker_bonus_score_reward_jingle_03", Define.Sound.Effect, 1.2f);
            StartCoroutine(NextScene(4f));
        }
    }
    private IEnumerator NextScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // 대기
        Managers.Scene.LoadScene(Define.Scene.StoryFour); // 다음 씬으로 전환
    }
    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (evt == Define.MouseEvent.Press) // 클릭 시작
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
    }
    private void ResetHold()
    {
        holdTime = 0f;
        target = null; // 클릭한 오브젝트 초기화
        isHolding = false;
        if (_bar != null)
        {
            _bar.ClosePopupUI();
            _bar = null;
        }
    }

    private void UpdateScoreText()
    {
        Get<Text>((int)Texts.ScoreText).GetComponent<Text>().text = score.ToString();
    }
}

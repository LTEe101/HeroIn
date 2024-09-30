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
    private float holdTime = 0f; // Ŭ�� �ð�
    private bool isHolding = false; // �� ������ �ִ��� ����
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

                _bar.SetBarImagePosition(target.name); // ������ ��ġ�� BarImage ��ġ ����
            }

            float fillAmount = holdTime / 2f;
            _bar.SetFillAmount(fillAmount);

            if (holdTime >= 2f && target != null && particleInstance == null && !hasScored)
            {
                hasScored = true;
                // ��ź ���󰡴� ȿ��
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
        {   // ������ ���� �� �ʱ�ȭ
            ResetHold();
        }
    }
    private IEnumerator PlayAnimationAndSpawnParticle(Animator animator, GameObject target)
    {
        GameObject ball = animator.gameObject;
        animator.SetTrigger("ShootTrigger");

        // �ִϸ��̼� ���� ��������
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength + 0.4f);
        if (particleInstance == null)
        {
            // ���� ȿ�� ����
            particleInstance = Instantiate(boom, target.transform.position, target.transform.rotation);
            particleInstance.SetActive(true);
            Managers.Sound.Play("ProEffect/Explosion_Fire_Gas/explosion_large_no_tail_02", Define.Sound.Effect, 0.2f);
        }

        Destroy(ball);
        
        // ����
        score++;
        UpdateScoreText();

        if (_bar != null)
        {
        // ������ UI ���ֱ�
        _bar.ClosePopupUI();
        _bar = null;
        }

        // �� ���ֱ�
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
        yield return new WaitForSeconds(waitTime); // ���
        Managers.Scene.LoadScene(Define.Scene.StoryFour); // ���� ������ ��ȯ
    }
    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (evt == Define.MouseEvent.Press) // Ŭ�� ����
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int mask = (1 << 7); // ���̾� ����ũ (7�� ���̾ Target���� ����)
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f, mask))
            {
                target = hit.collider.gameObject; // Ŭ���� ������Ʈ ����
                isHolding = true; // �� ������ ����
            }
        }
    }
    private void ResetHold()
    {
        holdTime = 0f;
        target = null; // Ŭ���� ������Ʈ �ʱ�ȭ
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

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
    private float holdTime = 0f; // Ŭ�� �ð�
    private bool isHolding = false; // �� ������ �ִ��� ����
    private GameObject target;
    private GameObject particleInstance;
    private GameObject boom;

    private GameObject[] cannonBalls;

    private Animator[] anims;
    public System.Action onFinished;

    // ���� ��� ���� ���� (���� 2�� ��� true�� ����)
    private bool isGameTwo = false;
    private EnemyManager enemyManager; // ���� 2���� ���� ���ŵ� �� ������ �߰��ϱ� ���� �ʿ�
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
            // ���� 2������ EnemyManager�� ��ȣ�ۿ�
            enemyManager = EnemyManager.Instance;
            if (enemyManager != null)
            {
                enemyManager.onEnemyDestroyed += OnEnemyDestroyed; // �� ���� �� ���� ����
            }
        }
        else
        {
            // ���� 1�� ���� (Ŭ�� �̺�Ʈ ó��)
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

    // ���� 2���� ���� ���ŵ� �� ���� �߰�
    private void OnEnemyDestroyed()
    {
        score++;
        UpdateScoreText();
        CheckGameEnd(); // ���ھ 3�� �� ���� �˾� ����
    }

    // ���� 1�� ���� Ŭ�� �� �ִϸ��̼� ó�� ����
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
                _gameBarBox.transform.SetParent(target.transform, false); // �θ�� ����

                if (_gameBarBox != null)
                {
                    Transform uiGameBar = _gameBarBox.transform.GetChild(0); // ù ��° �ڽ� (UI_Game_Bar)
                    if (uiGameBar != null)
                    {
                        _totalBar = uiGameBar.GetChild(0).GetComponent<Slider>(); // ù ��° �ڽ� (TotalBar)���� Slider ������
                    }
                }
            }

            if (_totalBar != null)
            {
                _totalBar.value = Mathf.Clamp01(holdTime / 2f); // 2�� ���� �����̴��� �� ������ ����
            }

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
        {
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
            Vector3 particlePosition = target.transform.position + new Vector3(0, 10, 5);
            particleInstance = Instantiate(boom, particlePosition, target.transform.rotation);
            particleInstance.SetActive(true);

            // ���� 1 ȿ���� ���
            AudioClip audioClip = Managers.Sound.GetOrAddAudioClip("ProEffect/Explosion_Fire_Gas/explosion_large_no_tail_02", Define.Sound.Effect);
            Managers.Sound.Play(audioClip, Define.Sound.Effect, 0.5f);
            Destroy(ball);
            Destroy(target);
            if (audioClip != null)
            {
                yield return new WaitForSeconds(audioClip.length); // ȿ���� ���̸�ŭ ���
            }
        }

        // ����
        score++;
        UpdateScoreText();

        if (_gameBarBox != null)
        {
            Destroy(_gameBarBox);
            _gameBarBox = null;
        }

        // �� ���ֱ�
        
        isHolding = false;
        hasScored = false;

        CheckGameEnd(); // ���ھ 3�� �� ���� �˾� ����
    }

    private void CheckGameEnd()
    {
        if (score >= 3) // ���ھ 3 �̻��̸� ���� �˾�
        {
            // Close the UI_Motion_State before showing the finish UI
            var motionStateUI = FindObjectOfType<UI_Motion_State>();
            if (motionStateUI != null)
            {
                motionStateUI.Close(); // Close the UI_Motion_State
            }

            TriggerEnemyDie();

            Managers.Sound.Play("ProEffect/Collectibles_Items_Powerup/points_ticker_bonus_score_reward_jingle_02", Define.Sound.Effect);

            Managers.UI.ShowPopupUI<UI_Game_Finish>(); // ���� �˾� ǥ��
            StartCoroutine(NextScene(5f)); // 5�� �Ŀ� ���� ������ ��ȯ
        }
    }

    private void TriggerEnemyDie()
    {
        // 'Enemy' �±׷� ��� �� ������Ʈ ã��
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // �� ���� Animator���� Die Ʈ���� Ȱ��ȭ
        foreach (GameObject enemy in enemies)
        {
            Animator animator = enemy.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Die"); // Die Ʈ���� Ȱ��ȭ
            }
        }
    }


    private IEnumerator NextScene(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // ���

        // ���� ������ ��ȯ
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
        if (!isGameTwo && evt == Define.MouseEvent.Press) // ���� 1�� ���� ����
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
        if (evt == Define.MouseEvent.PointerEnter)
        {
            // Outline ������Ʈ�� �ִ��� Ȯ��
            Outline outline = Managers.Input.HoveredObject.GetComponent<Outline>();
            if (outline != null)
            {
                // �ƿ����� ��带 OutlineAll ����
                outline.OutlineMode = Outline.Mode.OutlineAll;
            }
        }
        if (evt == Define.MouseEvent.PointerExit)
        {
            // Outline ������Ʈ�� �ִ��� Ȯ��
            Outline outline = Managers.Input.HoveredObject.GetComponent<Outline>();
            if (outline != null)
            {
                // �ƿ����� ��带 OutlineHidden�� ����
                outline.OutlineMode = Outline.Mode.OutlineHidden;
            }
        }

    }

    private void ResetHold()
    {
        holdTime = 0f;
        target = null; // Ŭ���� ������Ʈ �ʱ�ȭ
        isHolding = false;
        if (_gameBarBox != null)
        {
            Destroy(_gameBarBox); // GameBarBox �ν��Ͻ� ����
            _gameBarBox = null;
        }
        if (_effectSource != null)
        {
            _effectSource.Stop(); // ���� ����
            _effectSource = null;
        }
    }

    private void UpdateScoreText()
    {
        Get<Text>((int)Texts.ScoreText).GetComponent<Text>().text = score.ToString();
    }
}

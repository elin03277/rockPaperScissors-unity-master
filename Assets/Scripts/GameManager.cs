using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//GameManager for the rock paper scissors game, handles all game logic
//Patrick Ngo


public class GameManager : MonoBehaviour {

    //gameobject references neede for ui manipulations
    public GameObject playerChoiceImage;
    public GameObject aiChoiceImage;
    public GameObject playerScoreLabel;
    public GameObject aiScoreLabel;
    public GameObject winnerLabel;

    //choices
    private const string CHOICE_ROCK = "r";
    private const string CHOICE_PAPER = "p";
    private const string CHOICE_SCISSOR = "s";
    private const string CHOICE_NONE = "n";

    //winners
    private const int RESULT_NIL = 0;
    private const int RESULT_PLAYER = 1;
    private const int RESULT_AI = 2;

    //win timer
    public float WIN_TIMER_DEFAULT = 2.0f;
    public float winTimeLeft = 0.0f;
    private bool showWinner = false;

    //selections
    private string aiSelection = "";
    private string playerSelection = "";

    //0 = nil, 1 = player wins, 2 = ai wins
    private int winner = 0;

    //scores
    private double aiScore = 0;
    private double playerScore = 0;

    public string moves = "";

    // Predicts the next action of the player based on the previous two actions
    public class nGramPredictor
    {
        public Dictionary<string, string> data = new Dictionary<string, string>();
        public int nValue = 2 + 1;

        public string getNext(string rpsMoves)
        {
            int length = rpsMoves.Length;

            string actions;
            string predictions;

            int r = 0;
            int p = 0;
            int s = 0;

            // Every 3 items store the first two under actions and the last one on predictions
            if (length == 3)
            {
                actions = rpsMoves.Substring(0, 2);
                predictions = rpsMoves.Substring(rpsMoves.Length-1);

                Debug.Log(actions);
                Debug.Log(predictions);

                data.Add(actions, predictions);



                actions = "";
                predictions = "";
            }

            // If the past two actions match previous ones then predict the next action based on the highest occurance
            foreach (var v in data)
            {
                if (v.Key.ToString().Equals(rpsMoves.Substring(0, 2)))
                {
                    Debug.Log("VALUE: " + v.Key.ToString());

                    if (v.Value.ToString() == "r")
                    {
                        r++;
                    }
                    else if (v.Value.ToString() == "p")
                    {
                        p++;
                    }
                    else if (v.Value.ToString() == "s")
                    {
                        s++;
                    }
                } 
            }

            if(r != 0 || p != 0 || s != 0)
            {
                if(r > p && r > s)
                {
                    return "p";
                } else if(p > s && p > r)
                {
                    return "s";
                } else if(s > r && s > p)
                {
                    return "r";
                }
            }

            return "n";

        }


    }
    
	// Use this for initialization
	void Start () 
	{

		//hide images
		playerChoiceImage.SetActive (false);
		aiChoiceImage.SetActive (false);

		//hide win text
		winnerLabel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update()
	{
		if (!showWinner) {
		

            winTimeLeft -= Time.deltaTime;

            //countdown complete, finish win animation
            if (winTimeLeft <= 0)
            {
                showWinner = false;

                //reset win text timer
                winTimeLeft = WIN_TIMER_DEFAULT;


                //hide win text
                winnerLabel.SetActive(false);

                //hide images
                playerChoiceImage.SetActive(false);
                aiChoiceImage.SetActive(false);
            }
        }
	}

	private void AiSelection()
	{
        var ng = new nGramPredictor();
        string[] randomRPS = { "r", "p", "s" };
        System.Random rand = new System.Random();
        int index = rand.Next(randomRPS.Length);

        aiSelection = ng.getNext(moves); 
        Debug.Log("Selected: " + aiSelection);

        if(aiSelection == "n")
        {
            aiSelection = randomRPS[index];
        }

        if(moves.Length == 3)
        {
            moves = "";
        }

        //determine winner
        DetermineResult();

        //update scores
        UpdateScores();

        //update UI
        UpdateUI();

        //reset player choice
        playerSelection = CHOICE_NONE;
    }

	private void ResetGame()
	{
		aiScore = 0;
		playerScore = 0;

		SelectionMade ();
	}

	private void SelectionMade()
	{

		//ai makes a choice
		AiSelection ();

	}

	private void UpdateScores()
	{
		//increment scores
		if (winner == RESULT_PLAYER)
		{
			playerScore = playerScore + 1;
		}
		else if (winner == RESULT_AI)
		{
			aiScore = aiScore + 1;
		}
	}


	private void UpdateUI()
	{
		//show images
		playerChoiceImage.SetActive (true);
		aiChoiceImage.SetActive (true);

		//show winner text
		winnerLabel.SetActive(true);

		//Update selection sprites
		setSelectionSprite(aiSelection, aiChoiceImage);
		setSelectionSprite(playerSelection, playerChoiceImage);

		//Update score labels
		UILabel playerLabel = playerScoreLabel.GetComponent<UILabel>();
		UILabel aiLabel = aiScoreLabel.GetComponent<UILabel>();

		playerLabel.text = playerScore.ToString();
		aiLabel.text = aiScore.ToString();

		//show winner text
		UILabel winLabel = winnerLabel.GetComponent<UILabel>();

		string winnerText = "";
		int pointsWon = 0;
		if (winner == RESULT_AI) 
		{
			winnerText = "AI";
			pointsWon = 1;
			winLabel.text = winnerText + " wins! " + "+" + pointsWon;
		} 
		else if (winner == RESULT_PLAYER)
		{
			winnerText = "Player";
			pointsWon = 1;
			winLabel.text = winnerText + " wins! " + "+" + pointsWon;
		}
		else
		{
			winLabel.text = "TIE!";
		}

		showWinner = true;
	}

	private void setSelectionSprite(string selection, GameObject go)
	{
		//Update selection sprites
		UISprite sprite = go.GetComponent<UISprite>();

        if(selection == "p")
        {
            sprite.spriteName = "paper";
        }
        else if(selection == "r")
        {
            sprite.spriteName = "stone";
        } 
        else if(selection == "s") 
        {
            sprite.spriteName = "scissor";
        } 
        else 
        {
            go.SetActive (false);	
        }
	}

	//logic to determine the winner
	private void DetermineResult()
	{
        if (playerSelection == CHOICE_ROCK)
        {
            if(aiSelection == CHOICE_PAPER)
            {
                winner = RESULT_AI;
            }
            else if (aiSelection == CHOICE_ROCK)
            {
                winner = RESULT_NIL;

            }
            else if (aiSelection == CHOICE_SCISSOR)
            {
                winner = RESULT_PLAYER;

            }	
            else 
            {
                winner = RESULT_NIL;
			}
		}

        if (playerSelection == CHOICE_PAPER)
        {
            if (aiSelection == CHOICE_PAPER)
            {
                winner = RESULT_NIL;
            }
            else if (aiSelection == CHOICE_ROCK)
            {
                winner = RESULT_PLAYER;

            }
            else if (aiSelection == CHOICE_SCISSOR)
            {
                winner = RESULT_AI;

            }
            else
            {
                winner = RESULT_NIL;
            }
        }

		if (playerSelection == CHOICE_SCISSOR) 
		{

            if (aiSelection == CHOICE_PAPER)
            {
                winner = RESULT_PLAYER;
            }
            else if (aiSelection == CHOICE_ROCK)
            {
                winner = RESULT_AI;

            }
            else if (aiSelection == CHOICE_SCISSOR)
            {
                winner = RESULT_NIL;

            }
            else
            {
                winner = RESULT_NIL;
            }
		}

		if (playerSelection == CHOICE_NONE) 
		{
			winner = RESULT_AI;
		}
	}




    //Added to list of moves
	//Button receiver methods
	public void SelectRock()
	{
		playerSelection = CHOICE_ROCK;
        moves += CHOICE_ROCK;
        Debug.Log("Rock Selected: " + moves);
		SelectionMade ();
	}
	public void SelectScissors()
	{
		playerSelection = CHOICE_SCISSOR;
        moves += CHOICE_SCISSOR;
        Debug.Log("Scissor Selected " + moves);
        SelectionMade ();
	}
	public void SelectPaper()
	{
		playerSelection = CHOICE_PAPER;
        moves += CHOICE_PAPER;
        Debug.Log("Paper Selected " + moves);
        SelectionMade ();
	}


}

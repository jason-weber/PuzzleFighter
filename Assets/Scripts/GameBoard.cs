using UnityEngine;
using System.Collections;

public class GameBoard : MonoBehaviour {
	
	private GameObject[,] board;
	public int BOARD_WIDTH = 7;
	public int BOARD_HEIGHT = 7;
	public GameObject[] prefabs;
	private Block selectedBlock;

	// Use this for initialization
	void Start () {
		this.board = new GameObject[BOARD_HEIGHT,BOARD_WIDTH];
		this.fillBoard();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Checks for empty spots on the board and fills it accordingly
	void fillBoard() {
		Vector3 startPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * .25f, Screen.height * .1f));
		for(int y = 0; y < BOARD_HEIGHT; y++) {
			for(int x = 0; x < BOARD_WIDTH; x++) {
				if(board[y,x] == null){
					int randIndex = Random.Range(0, prefabs.Length);
					GameObject block = Instantiate(prefabs[randIndex], new Vector3(startPoint.x + x, startPoint.y + y), Quaternion.identity) as GameObject;
					Block blockScript = block.GetComponent<Block>();
					blockScript.selectEvent += new SelectEventHandler(selectBlock);
					blockScript.x = x;
					blockScript.y = y;
					board[y, x] = block;
				}
			}
		}
	}

	void selectBlock(Block sender) {
		Debug.Log("Selected block: (" + sender.x + ", " + sender.y + ")");
		if(selectedBlock == null) {
			selectedBlock = sender;
			sender.transform.position += Vector3.up * .1f;
		} else {
			selectedBlock.transform.position += Vector3.down * .1f;
			if(selectedBlock == sender) {
				selectedBlock = null;
			} else {
				if(checkMove(selectedBlock, sender)) {
					swapBlocks(selectedBlock, sender);
					checkForMatches(selectedBlock);
					checkForMatches(sender);
				}
			}
			selectedBlock = null;
		}
	}

	private bool isMatch(Block first, int x, int y) {
		return x >= 0 && x < BOARD_WIDTH && 
			y >= 0 && y < BOARD_HEIGHT && 
			board[y, x] != null && first.blockType == board[y,x].GetComponent<Block>().blockType;
	}

	void checkForMatches(Block block) {
		// check left
		bool oneLeft = isMatch(block, block.x - 1, block.y);
		bool twoLeft = oneLeft && isMatch(block, block.x - 2, block.y);
		// check right
		bool oneRight = isMatch(block, block.x + 1, block.y);
		bool twoRight = oneRight && isMatch(block, block.x + 2, block.y);
		// check up
		bool oneUp = isMatch(block, block.x, block.y + 1);
		bool twoUp = oneUp && isMatch(block, block.x, block.y + 2);
		// check down
		bool oneDown = isMatch(block, block.x, block.y - 1);
		bool twoDown = oneDown && isMatch(block, block.x, block.y - 2);

		bool horizontalMatch = oneLeft && twoLeft || oneLeft && oneRight || oneRight && twoRight;
		bool verticalMatch = oneUp && twoUp || oneUp && oneDown || oneDown && twoDown;

		if(horizontalMatch) {
			if(oneLeft) {
				GameObject.Destroy(board[block.y, block.x - 1]);
				if(twoLeft) {
					GameObject.Destroy(board[block.y, block.x - 2]);
				}
			}
			if(oneRight) {
				GameObject.Destroy(board[block.y, block.x + 1]);
				if(twoRight) {
					GameObject.Destroy(board[block.y, block.x + 2]);
				}
			}
		}
		if(verticalMatch) {
			if(oneUp) {
				GameObject.Destroy(board[block.y + 1, block.x]);
				if(twoUp) {
					GameObject.Destroy(board[block.y + 2, block.x]);
				}
			}
			if(oneDown) {
				GameObject.Destroy(board[block.y - 1, block.x]);
				if(twoDown) {
					GameObject.Destroy(board[block.y - 2, block.x]);
				}
			}
		}
		if(verticalMatch || horizontalMatch) {
			GameObject.Destroy(block.gameObject);
		}
	}

	// Checks if a given move is valid or not
	bool checkMove(Block first, Block second) {
		Debug.Log("Tried to make move: (" + first.x + ", " + first.y + ") (" + second.x + ", " + second.y + ")");
		int diffX = (int)Mathf.Abs(first.x - second.x);
		int diffY = (int)Mathf.Abs(first.y - second.y);
		return (diffX == 0 && diffY == 1) || (diffX == 1 && diffY == 0);
	}

	void swapBlocks(Block first, Block second) {
		// Swap value on board
		Block temp = first;
		board[first.y, first.x] = second.gameObject;
		board[second.y, second.x] = temp.gameObject;

		// Swap x's and y's
		int tempX = first.x;
		int tempY = first.y;
		first.x = second.x;
		first.y = second.y;
		second.x = tempX;
		second.y = tempY;

		// Swap position
		Vector3 tempPos = first.transform.position;
		first.transform.position = second.transform.position;
		second.transform.position = tempPos;
	}
}

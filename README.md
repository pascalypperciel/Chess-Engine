
# Chess-Engine

This is a personal project of mine. It all started with simply creating a chess game on unity which took me an afternoon of my time. However, I quickly became absorbed in the complexity that creating a Chess AI can offer.

This project has proved to be a fantastic opportunity to learn about what I love: algorithms, data structure, time & space optimization, machine learning, and more.

<details>
<summary> Entry #1: January 21, 2023</summary>

**Introduction**

After my first university semester, on December 27th 2023, I wanted a Christmas break project. I decided to program a chess game as I enjoy the game and thought it was going to be a challenge as chess is a somewhat complex. However, after a day and a half I already had a working game of chess. I used Unity (and therefore C#) to make it. However, I was a bit disappointed as I expected a tougher challenge. That’s when I decided to build a chess engine so my chess game could be played without the need of a human adversary. Now that was the challenge I was looking for.

**The Journey Begins – 1st version of the engine.**

Still on C#, I started my first artificial intelligence project to build a chess engine from scratch. More precisely, I wanted to attempt making my chess AI with machine learning. Using Perft, I made sure that my code and algorithms could properly find all legal moves of a given position. 
Using Unity’s Machine Learning Agents (ML-Agents) that runs on Python, I started training my AI to play chess. At the time, I would simply let the ML-Agent move against a random move generator. After letting in run for hours it was clear that I had to start letting my AI play against a better opponent than a random move generator if I wanted it to learn decently enough. Then, I started working on another AI that ran on algorithms and code instead of machine learning so I could ultimately use it to train my ML-Agent.
This was my first experience using algorithms. I started by implementing Minimax algorithm, Alpha-beta pruning and move ordering algorithms. Those algorithms were a great start. Although, I started running into limitations. First of all, chess engines are very resource-demanding programs. They need to calculate millions of moves and position in order to be half-decent. And I was running all of that on my base code that I initially did. Admittedly, my code was not optimized for that at all. When I started using the algorithms, they were so poorly optimized that it took a few seconds to spit out a move at only depth 4. Changes had to be made as the very foundation of my project wasn’t built with the idea of chess engine in mind. So I decided I was going to do this right.

**The Future of My Chess Engine**

I knew that radical changes had to be made. After analysing my scripts, I noticed a bunch of malpractices and improvements to be made, mainly: too frequent use of GetComponent<> function, there was probably a better algorithm to find all the legal moves, there was too many for loops or foreach loops that was iterating through entire list of moves when I could store data in arrays for more efficient access to some information (such as finding the pinned pieces on a board, or what pieces or defended by allied pieces). Therefore, I decided to do a fresh start on this AI and do this part on C++. C# will always be a part of my project as I like the ML-Agents library and will keep using it for the machine learning part of my program. However, I will redo all the algorithms and greatly optimize everything on C++.
Chess engines are far more complex that I originally thought. However, this project as taught me so much in time and space optimization, AI (algorithms and machine learning), Object-oriented programing and is expanding my knowledge in C++, C# and Python. I love this challenge and will keep on working in my free time.
</details>

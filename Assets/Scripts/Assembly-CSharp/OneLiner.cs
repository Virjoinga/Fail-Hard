using System.Collections.Generic;
using UnityEngine;

public class OneLiner : MonoBehaviour
{
	public UILabel Label;

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		List<string> list = new List<string>();
		list.Add("How does the crew know the ramp is in the right place?\n\n- The stuntman crashes after the jump instead of before it.");
		list.Add("How many people does it take to make a stunt happen?\n- Eleven. A stuntman for the jump and ten others to complain how they could’ve done it better.");
		list.Add("Johnny was really disappointed to find out that\n “doing donuts” had nothing to do with the dessert.");
		list.Add("What do you call a motorist who can’t drive?\n\n- A stuntman.");
		list.Add("Not to say that the crew’s mechanic has toxic attitude, but...\n Once a snake bit him and after three grueling weeks,\n the poor serpent died.");
		list.Add("There are no accidents, just a list of things Johnny wants to try next.");
		list.Add("No matter what people say about these stunts,\n Johnny doesn’t actually cheat death.\n\n He wins fair and square.");
		list.Add("You don’t have to be crazy to be a stuntman, but it definitely helps.");
		list.Add("What is the biggest difference between a computer\n and a stuntman?\n\n- The computer stops after a crash.");
		list.Add("How do you know the manager is about to lie?\n\n- He opens his mouth.");
		list.Add("Johnny is so lucky he once won a poker match\n with a four of hearts, a joker and two uno cards.\n And he thought they were playing Go Fish.");
		list.Add("The tower of Pisa used to stand straight.\n Then Johnny visited Italy...");
		list.Add("What do you call a beautiful woman\n on a stuntman’s arm?\n\n- A tattoo.");
		list.Add("Johnny is excited that with his recent earnings\n he can finally get a moped license.");
		list.Add("Even if Johnny suggests,\n Plan B is not just doubling the speed from Plan A.");
		list.Add("Everything that can go wrong, will go wrong, especially when Johnny is involved.");
		list.Add("A rule of thumb: if a stunt looks stupid, but works,\n then it’s not stupid.");
		list.Add("When in doubt, speed up.\n The jump is longer than it seems.");
		list.Add("Johnny is no longer allowed to have energy drinks, parachutes or second thoughts.");
		list.Add("A good stuntman can jump from any height;\n a great one can do it more than once.");
		list.Add("There are three ways to do things, the right way,\n the wrong way and Johnny’s way.");
		list.Add("Success occurs when no one sees,\n the failures happen when the manager is watching.");
		list.Add("What do you call a stuntman crashing the moped hard enough to break off the wheels?\n\n- Typical...");
		list.Add("What's the biggest difference between a battery\n and the team’s mechanic?\n\n- The battery has a positive side.");
		list.Add("Johnny finally figured out\n that some things are better left unsaid.\n Unfortunately he has no idea what those things are...");
		list.Add("Johnny can handle any pain!\n\n ...At least until it hurts.");
		list.Add("The road to disaster is paved with good ideas.");
		list.Add("The manager always means what he says.\n\n But he doesn’t necessarily mean to say it out loud.");
		list.Add("Johnny had to get a weapon permit for karaoke.");
		list.Add("There are no limits to what Johnny can accomplish!\n\n ...when he is supposed to be doing something else.");
		list.Add("They say misery loves company,\n but not even misery wants to stick around Johnny for very long.");
		list.Add("Never play dumb with Johnny.\n That’s a game he can’t lose.");
		list.Add("If ignorance is bliss, then stuntmen must be\n the happiest people alive.");
		list.Add("Honesty is the best policy,\n as long as you agree with the manager.");
		list.Add("Johnny’s manager is an expert at multitasking.\n He can ignore countless things at once.");
		list.Add("With this stuntman gig,\n Johnny has really found the key to success.\n\n Now if he could just find the lock...");
		list.Add("First rule of stunt safety:\n\n Stay out of Johnny’s way. Seriously.");
		list.Add("Two wrongs don't make a right, but Johnny is determined to keep trying.");
		list.Add("Some people never learn.\n That’s why we have stuntmen.");
		list.Add("For the record, the mechanic is not lazy,\n he is energy efficient.");
		list.Add("Whats the difference between a stuntman and a large pizza?\n\nA large pizza can feed a family of four");
		list.Add("Give Johnny a penny for his thoughts, you’ll get change.");
		list.Add("If you’re living on the edge, make sure you’re wearing a seat belt.");
		list.Add("A manager is someone who takes a subject you understand and makes it sound confusing.");
		list.Add("Johnny wondered why the frisbee was getting bigger, and then it hit him.");
		list.Add("What do Johnny and a slinky have in common?\n\nThey're both fun to watch tumble down the stairs.");
		list.Add("Johnny didn't sleep very well last night. So in the morning he put some energy drink in his coffee... Johnny was half way up the ramp when he realized he had fogotten his moped.");
		list.Add("Johnny never makes mistakes... He thought he did once; but he was wrong.");
		list.Add("When Johnny was small, he wanted to be a stuntman when he grew up. But his mom told him that he couldn't be both.");
		list.Add("There were two people walking down the street. One was a stuntman. The other didn't have any money either.");
		list.Add("What do you call a stuntman in a suit?\n\nThe Defendant");
		Label.text = list[Random.Range(0, list.Count)];
	}
}

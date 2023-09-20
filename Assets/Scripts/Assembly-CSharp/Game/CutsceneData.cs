using System.Collections.Generic;

namespace Game
{
	public class CutsceneData
	{
		public CutsceneParameters m_parameters;

		public string Id
		{
			get
			{
				return m_parameters.Id;
			}
			set
			{
				m_parameters.Id = value;
			}
		}

		public List<CutsceneFrame> Frames
		{
			get
			{
				return m_parameters.Frames;
			}
			set
			{
				m_parameters.Frames = value;
			}
		}

		public List<GameEvent> Preconditions
		{
			get
			{
				return m_parameters.Preconditions;
			}
			set
			{
				m_parameters.Preconditions = value;
			}
		}

		public string AllowedTransition
		{
			get
			{
				return m_parameters.AllowedTransition;
			}
			set
			{
				m_parameters.AllowedTransition = value;
			}
		}

		public bool SingleShot
		{
			get
			{
				return m_parameters.SingleShot;
			}
			set
			{
				m_parameters.SingleShot = value;
			}
		}

		public CutsceneData(CutsceneParameters parameters)
		{
			m_parameters = parameters;
		}
	}
}

using System;
using UnityEngine;

namespace DictionalyAddon
{
	/// <summary>
	/// シリアライズ可能な KeyValuePair
	/// </summary>
	[Serializable]
	public abstract class SerializableKeyValuePair<TKey, TValue>
	{
		[SerializeField] private TKey   m_key   = default;
		[SerializeField] private TValue m_value = default;

		/// <summary>
		/// キーを返します
		/// </summary>
		public TKey Key{
			get { return m_key; }
			set { m_key = value; }
		}

		/// <summary>
		/// 値を返します
		/// </summary>
		public TValue Value{
			get { return m_value; }
			set { m_value = value; }
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected SerializableKeyValuePair()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected SerializableKeyValuePair( TKey key, TValue value )
		{
			m_key   = key;
			m_value = value;
		}
	}
}
//
// ServiceStack.Redis: ECMA CLI Binding to the Redis key-value storage system
//
// Authors:
//   Demis Bellot (demis.bellot@gmail.com)
//
// Copyright 2010 Liquidbit Ltd.
//
// Licensed under the same terms of reddis and ServiceStack: new BSD license.
//

using System;
using System.Collections.Generic;

namespace ServiceStack.Redis
{
	public interface IRedisNativeClient 
		: IDisposable
	{
		Dictionary<string, string> Info { get; }
		int Db { get; set; }
		void Set(string key, byte[] value);
		int SetNX(string key, byte[] value);
		byte[] Get(string key);
		byte[] GetSet(string key, byte[] value);
		int Del(string key);
		int Incr(string key);
		int IncrBy(string key, int count);
		int Decr(string key);
		int DecrBy(string key, int count);
		string RandomKey();
		bool Rename(string oldKeyname, string newKeyname);
		int Expire(string key, int seconds);
		int ExpireAt(string key, long unixTime);
		int Ttl(string key);

		string Save();
		void BgSave();
		void Shutdown();
		void Quit();
		void FlushDb();
		void FlushAll();

		byte[][] Sort(string listOrSetId, int startingFrom, int endingAt, bool sortAlpha, bool sortDesc);


		byte[][] SMembers(string setId);
		void SAdd(string setId, byte[] value);
		void SRem(string setId, byte[] value);
		byte[] SPop(string setId);
		void SMove(string fromSetId, string toSetId, byte[] value);
		int SCard(string setId);
		int SIsMember(string setId, byte[] value);
		byte[][] SInter(params string[] setIds);
		void SInterStore(string intoSetId, params string[] setIds);
		byte[][] SUnion(params string[] setIds);
		void SUnionStore(string intoSetId, params string[] setIds);
		byte[][] SDiff(string fromSetId, params string[] withSetIds);
		void SDiffStore(string intoSetId, string fromSetId, params string[] withSetIds);
		byte[] SRandMember(string setId);

	
		byte[][] LRange(string listId, int startingFrom, int endingAt);		
		void RPush(string listId, byte[] value);
		void LPush(string listId, byte[] value);
		void LTrim(string listId, int keepStartingFrom, int keepEndingAt);
		int LRem(string listId, int removeNoOfMatches, byte[] value);
		int LLen(string listId);
		byte[] LIndex(string listId, int listIndex);
		void LSet(string listId, int listIndex, byte[] value);
		byte[] LPop(string listId);
		byte[] RPop(string listId);
		byte[] RPopLPush(string fromListId, string toListId);


		int ZAdd(string setId, double score, byte[] value);
		int ZRem(string setId, byte[] value);
		double ZIncrBy(string setId, double incrBy, byte[] value);
		int ZRank(string setId, byte[] value);
		int ZRevRank(string setId, byte[] value);
		byte[][] ZRange(string setId, int min, int max);
		byte[][] ZRangeWithScores(string setId, int min, int max);
		byte[][] ZRevRange(string setId, int min, int max);
		byte[][] ZRevRangeWithScores(string setId, int min, int max);
		byte[][] ZRangeByScore(string setId, double min, double max, int? skip, int? take);
		byte[][] ZRangeByScoreWithScores(string setId, double min, double max, int? skip, int? take);
		byte[][] ZRevRangeByScore(string setId, double min, double max, int? skip, int? take);
		byte[][] ZRevRangeByScoreWithScores(string setId, double min, double max, int? skip, int? take);
		int ZRemRangeByRank(string setId, int min, int max);
		int ZCard(string setId);
		double ZScore(string setId, byte[] value);
		int ZUnion(string intoSetId, params string[] setIds);
		int ZInter(string intoSetId, params string[] setIds);

		int HSet(string hashId, string key, byte[] value);
		byte[] HGet(string hashId, string key);
		int HDel(string hashId, string key);
		bool HExists(string hashId, string key);
		int HLen(string hashId);
		byte[][] HKeys(string hashId);
		byte[][] HValues(string hashId);
		byte[][] HGetAll(string hashId);
	}
}
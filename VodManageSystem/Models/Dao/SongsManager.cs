using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VodManageSystem.Models.DataModels;

namespace VodManageSystem.Models.Dao
{
    /// <summary>
    /// a service of Song manager that maintains Song table and its related tables in database
    /// </summary>
    public class SongsManager : IDisposable
    {
        // private members
        private readonly KtvSystemDBContext _context;

        // public members

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VodManageSystem.DOA.SongManager"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        public SongsManager(KtvSystemDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifies the song.
        /// </summary>
        /// <returns>The song.</returns>
        /// <param name="song">Song.</param>
        private async Task<int> VerifySong(Song song)
        {
            Console.WriteLine("LanguageId = " + song.LanguageId);
            Console.WriteLine("Singer1Id = " + song.Singer1Id);
            Console.WriteLine("Singer2Id = " + song.Singer2Id);
            int result = 1; // valid by verification 
            if (song.LanguageId >= 0)
            {
                Language? lang = await _context.Language.Where(x => x.Id == song.LanguageId).SingleOrDefaultAsync();
                if (lang == null)
                {
                    // no Language.Id found
                    return ErrorCodeModel.LanguageNoNotFound;                    
                }
                song.LanguageId = lang.Id;
            }
            else
            {
                // language id. has to be specified
                return ErrorCodeModel.LanguageIdIsSpecified;            
            }
            if (song.Singer1Id >= 0)
            {
                Singer? sing1 = await _context.Singer.Where(x => x.Id == song.Singer1Id).SingleOrDefaultAsync();
                if (sing1 == null)
                {
                    // no Singer.SingNo for singer1 found
                    return ErrorCodeModel.Singer1NoNotFound;
                }
                song.Singer1Id = sing1.Id;
            }
            else
            {
                // song.Singer1Id not specified
                return ErrorCodeModel.Singer1IdIsNotSpecified;            
            }
            if (song.Singer2Id >= 0)
            {
                Singer? sing2 = await _context.Singer.Where(x => x.Id == song.Singer2Id).SingleOrDefaultAsync();
                if (sing2 == null)
                {
                    // song.Singer2Id not specified
                    return ErrorCodeModel.Singer2IdIsNotSpecified;                
                }
                song.Singer2Id = sing2.Id;
            }
            else
            {
                // no Singer.SingNo for singer2 found
                return ErrorCodeModel.Singer2NoNotFound;
            }
            /*  not for local KTV anymore
            if (song.VodYn == "Y")
            {
                // must have VodNo and Pathname
                if (string.IsNullOrEmpty(song.VodNo))
                {
                    // Vod No. is empty
                    return ErrorCodeModel.VodNoOfSongIsEmpty;                    
                }
                if (string.IsNullOrEmpty(song.Pathname))
                {
                    // Path name is empty
                    return ErrorCodeModel.PathnameOfVodNoIsEmpty;                    
                }
            }
            */
            if (song.SNumWord <= 0)
            {
                // number of words cannot be less than 0 or equal to 0
                return ErrorCodeModel.NumOfWordsLessOrEqualToZero;
            }

            return result;
        }

        /// <summary>
        /// Gets the total page of song table.
        /// </summary>
        /// <returns>The total page of song table.</returns>
        private int[] GetTotalRecordsAndPages(int pageSize)  // by a condition
        {
            int[] result = [0, 0];

            if (pageSize <= 0)
            {
                Console.WriteLine("GetTotalRecordsAndPages.pageSize cannot be less than 0.");
                return result;
            }

            int totalRecords = _context.Song.Count();        
            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) < totalRecords)
            {
                totalPages++;
            }
            Console.WriteLine("GetTotalRecordsAndPages.totalRecords = " + totalRecords);        
            Console.WriteLine("GetTotalRecordsAndPages.totalPages = " + totalPages);

            result[0] = totalRecords;
            result[1] = totalPages;

            return result;
        }

        /// <summary>
        /// Gets the total page of song table.
        /// </summary>
        /// <returns>The total page of IQueryable<Song>.</returns>
        private static int[] GetTotalRecordsAndPages(IQueryable<Song> totalSongs, int pageSize)  // by a condition
        {
            int[] result = [0, 0];

            if (pageSize <= 0)
            {
                Console.WriteLine("GetTotalRecordsAndPages.pageSize cannot be less than 0.");
                return result;
            }

            int totalRecords = totalSongs.Count();
            int totalPages = totalRecords / pageSize;
            if ((totalPages * pageSize) < totalRecords)
            {
                totalPages++;
            }
            Console.WriteLine("GetTotalRecordsAndPages.totalRecords = " + totalRecords);
            Console.WriteLine("GetTotalRecordsAndPages.totalPages = " + totalPages);

            result[0] = totalRecords;
            result[1] = totalPages;

            return result;
        }

        private static void UpdateStateOfRequest(StateOfRequest mState, Song? firstSong, int pageNo, int pageSize, int totalRecords, int totalPages, bool isFind=false)
        {
            mState.CurrentPageNo = pageNo;
            mState.PageSize = pageSize;
            mState.TotalRecords = totalRecords;
            mState.TotalPages = totalPages;
            if (firstSong != null)
            {
                mState.FirstId = firstSong.Id;
                if (!isFind)
                {
                    // mState.OrgId = mState.FirstId;
                }
            }
            else
            {
                mState.OrgId = 0;
                mState.OrgNo = "";
                mState.FirstId = 0;
            }
        }

        private IQueryable<Song> GetAllSongsIQueryableBasicFilter(StateOfRequest mState)
        {          
            Console.WriteLine("GetAllSongsIQueryableBasicFilter");  
            return GetAllSongsIQueryableBasic(mState, false);
         
        }

        private IQueryable<Song> GetAllSongsIQueryableBasic(StateOfRequest mState, bool needBaseFilter)
        {
            Console.WriteLine("GetAllSongsIQueryableBasic");
            IQueryable<Song> emptySongs = Enumerable.Empty<Song>().AsQueryable();
            if (mState == null)
            {
                return emptySongs;
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("pageSize cannot be less than 0.");
                return emptySongs;
            }

            IQueryable<Song> totalSongs = _context.Song.Include(x => x.Language).Include(x => x.Singer1).Include(x => x.Singer2);
            if (needBaseFilter)
            {
                // add basic filter for U2bKaraOke Android App
                totalSongs = totalSongs.Where(x => (x.MMpeg == "00") && (x.NMpeg == "00"));   // YouTube video link is ready
            }

            IQueryable<Song> songs;

            // guard against null OrderBy to avoid NullReferenceException
            string orderByParam = (mState.OrderBy ?? string.Empty).Trim();        
            if (orderByParam == "")
            {
                songs = totalSongs;
            }
            else if (orderByParam.Equals("SongNo", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("SongNa", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("NumWordsSongNa", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.SNumWord).ThenBy(x => x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("VodNo", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.VodNo).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("LangSongNa", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Language == null)
                                  .ThenBy(x => (x.Language != null ? x.Language.LangNo : "") + x.SongNa).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("Singer1Na", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Singer1 == null)
                                  .ThenBy(x => (x.Singer1 != null ? x.Singer1.SingNa : "")).ThenBy(x => x.SongNo);
            }
            else if (orderByParam.Equals("Singer2Na", StringComparison.OrdinalIgnoreCase))
            {
                songs = totalSongs.OrderBy(x => x.Singer2 == null)
                                  .ThenBy(x => (x.Singer2 != null ? x.Singer2.SingNa : "")).ThenBy(x => x.SongNo);
            }
            else
            {
                // not inside range of roder by
                songs = emptySongs;   // empty lsit
            }

            return songs ?? emptySongs;
        }

        private static IQueryable<Song> GetSongsIQueryableAddFilter(IQueryable<Song> originalSongs, string filter)
        {
            Console.WriteLine("GetSongsIQueryableAddFilter.filter = " + filter);
            IQueryable<Song> emptySongs = Enumerable.Empty<Song>().AsQueryable();
            IQueryable<Song> songs = originalSongs ?? emptySongs;
            
            Console.WriteLine("GetSongsIQueryableAddFilter.originalSongs = " + originalSongs);
            if ((originalSongs != null) && (!string.IsNullOrEmpty(filter)))
            {
                // Split by '+' and remove empty entries to handle trailing or double '+'
                string[] filterParts = filter.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                // Iterate through parts in pairs (FieldName + Value)
                Console.WriteLine("GetSongsIQueryableAddFilter.filterParts.Length = " + filterParts.Length);
                for (int i = 0; i < filterParts.Length - 1; i += 2)
                {
                    string fieldName = filterParts[i].Trim();
                    string fieldSubValue = filterParts[i + 1].Trim();
                    Console.WriteLine("GetSongsIQueryableAddFilter.fieldName = " + fieldName);
                    Console.WriteLine("GetSongsIQueryableAddFilter.fieldSubValue  = " + fieldSubValue);
                    if (!string.IsNullOrEmpty(fieldSubValue))
                    {
                        if (fieldName.Equals("SongNo", StringComparison.OrdinalIgnoreCase))
                        {
                            songs = songs.Where(x => x.SongNo != null && x.SongNo.Contains(fieldSubValue));
                        }
                        else if (fieldName.Equals("SongNa", StringComparison.OrdinalIgnoreCase))
                        {
                            songs = songs.Where(x => x.SongNa != null && x.SongNa.Contains(fieldSubValue));
                        }
                        // Add more fields here as needed, use the string "VideoReady" for the u2bkaraoke Android App filter
                        else if (fieldName.Equals("VideoReady", StringComparison.OrdinalIgnoreCase)) {
                            Console.WriteLine("GetSongsIQueryableAddFilter.Video Ready Only");
                            songs = songs.Where( x => (x.MMpeg == "00") && (x.NMpeg == "00"));
                        }                        
                    }
                }
            }

            return songs ?? emptySongs;
        }

        private IQueryable<Song> GetAllSongsIQueryable(StateOfRequest mState)
        {
            Console.WriteLine("GetAllSongsIQueryable.mState.QueryCondition = " + mState.QueryCondition);
            IQueryable<Song> songs = GetAllSongsIQueryableBasicFilter(mState);
            songs = GetSongsIQueryableAddFilter(songs, mState.QueryCondition);

            return songs;
        }

        // end of private methods

        // public methods
        public List<Song> GetAllSongs(StateOfRequest mState)
        {
            if (mState == null)
            {
                return [];    // return empty list
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("GetAllSongs.pageSize cannot be less than 0.");
                return [];
            }

            mState.CurrentPageNo = -100; // present to get all songs
            List<Song> totalSongs = GetOnePageOfSongs(mState);

            return totalSongs;
        }

        // No filter
        public List<Song> GetOnePageOfSongs(StateOfRequest mState)
        {
            Console.WriteLine("GetOnePageOfSongs");
            return GetOnePageOfSongs(mState, "");
        }

        // filter with number of words, this functionalitry is only for WebApi so far
        public List<Song> GetOnePageOfSongs(StateOfRequest mState, string numWords)
        {            
            Console.WriteLine($"GetOnePageOfSongs.numWords = {numWords}");
            if (mState == null)
            {
                return [];
            }
            int pageSize = mState.PageSize;
            Console.WriteLine($"GetOnePageOfSongs.pageSize = {pageSize}");
            if (pageSize <= 0)
            {
                Console.WriteLine("GetOnePageOfSongs.pageSize cannot be less than 0.");
                return [];
            }

            int pageNo = mState.CurrentPageNo;
            Console.WriteLine($"GetOnePageOfSongs.pageNo = {pageNo}");

            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);        
            if (totalSongs == null)
            {
                Console.WriteLine("GetOnePageOfSongs.totalSongs = null");
                return [];
            }

            if (!string.IsNullOrEmpty(numWords)) {
                int number;
                if (Int32.TryParse(numWords, out number))
                {
                    Console.WriteLine($"GetOnePageOfSongs parsed: {number}");
                }
                else
                {
                    Console.WriteLine("GetOnePageOfSongs.Conversion failed. The string is not a valid integer.");
                    // 'number' will be 0 here if the conversion fails.
                    number = 1;
                }
                if (number < 1) number = 1;
                totalSongs = totalSongs.Where(x => (x.SNumWord == number));
            }

            int[] returnNumbers = GetTotalRecordsAndPages(totalSongs, pageSize);
            int totalRecords = returnNumbers[0];            
            int totalPages = returnNumbers[1];
            Console.WriteLine($"GetOnePageOfSongs.totalRecords = {totalRecords}");
            Console.WriteLine($"GetOnePageOfSongs.totalPages = {totalPages}");

            // bool getAll = false; // removed on 2018-11-26
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
            }
            else
            {
                if (pageNo < 1)
                {
                    pageNo = 1;
                }
                else if (pageNo > totalPages)
                {
                    pageNo = totalPages;
                }
            }
            Console.WriteLine($"GetOnePageOfSongs.pageNo = {pageNo}");

            int recordNum = (pageNo - 1) * pageSize;
            Console.WriteLine($"GetOnePageOfSongs.recordNum = {recordNum}");
            if (recordNum < 0) recordNum = 0;
            List<Song> songs = totalSongs.Skip(recordNum).Take(pageSize).ToList();
            Console.WriteLine($"GetOnePageOfSongs.songs.Count = {songs.Count}");

            // avoid passing null for firstSong to UpdateStateOfRequest
            // var firstSong = songs.FirstOrDefault() ?? new Song();
            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        // filter with number of words, this functionalitry is only for WebApi so far
        public List<Song> GetOnePageOfSongsWithFilter(StateOfRequest mState)
        {            
            if (mState == null)
            {
                Console.WriteLine($"GetOnePageOfSongsWithFilter.mState is null");
                return [];
            }
            int pageSize = mState.PageSize;
            Console.WriteLine($"GetOnePageOfSongsWithFilter.pageSize = {pageSize}");
            if (pageSize <= 0)
            {
                Console.WriteLine("GetOnePageOfSongsWithFilter.pageSize cannot be less than 0.");
                return [];
            }
            int pageNo = mState.CurrentPageNo;
            Console.WriteLine($"GetOnePageOfSongsWithFilter.pageNo = {pageNo}");

            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);        
            if (totalSongs == null)
            {
                Console.WriteLine("GetOnePageOfSongs.totalSongs = null");
                return [];
            }

            int[] returnNumbers = GetTotalRecordsAndPages(totalSongs, pageSize);
            int totalRecords = returnNumbers[0];            
            int totalPages = returnNumbers[1];
            Console.WriteLine($"GetOnePageOfSongsWithFilter.totalRecords = {totalRecords}");
            Console.WriteLine($"GetOnePageOfSongsWithFilter.totalPages = {totalPages}");

            // bool getAll = false; // removed on 2018-11-26
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
            }
            else
            {
                if (pageNo < 1)
                {
                    pageNo = 1;
                }
                else if (pageNo > totalPages)
                {
                    pageNo = totalPages;
                }
            }
            Console.WriteLine($"GetOnePageOfSongsWithFilter.pageNo = {pageNo}");

            int recordNum = (pageNo - 1) * pageSize;
            Console.WriteLine($"GetOnePageOfSongsWithFilter.recordNum = {recordNum}");
            if (recordNum < 0) recordNum = 0;
            List<Song> songs = totalSongs.Skip(recordNum).Take(pageSize).ToList();
            Console.WriteLine($"GetOnePageOfSongsWithFilter.songs.Count = {songs.Count}");

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        public List<Song> GetOnePageOfSongsBySingerId(StateOfRequest mState, int singerId, bool isWebAPI)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            totalSongs = totalSongs.Where(x => (x.Singer1Id == singerId) || (x.Singer2Id == singerId));        
            int[] returnNumbers = GetTotalRecordsAndPages(totalSongs, pageSize);
            int totalRecords = returnNumbers[0];            
            int totalPages = returnNumbers[1];

            int pageNo = mState.CurrentPageNo;            
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all songs
                // getAll = true;   // removed on 2018-11-26
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
            }
            else
            {
                if (!isWebAPI)
                {
                    if (pageNo < 1)
                    {
                        pageNo = 1;
                    }
                    else if (pageNo > totalPages)
                    {
                        pageNo = totalPages;
                    }
                }
            }

            int recordNum = (pageNo - 1) * pageSize;
            if (recordNum < 0) recordNum = 0;
            List<Song> songs = totalSongs.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        public List<Song> GetOnePageOfSongsByLanguageIdNumOfWords(StateOfRequest mState, int languageId, int numOfWords, bool isWebAPI)
        {
            Console.WriteLine("GetOnePageOfSongsByLanguageIdNumOfWords");
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("pageSize cannot be less than 0.");
                return new List<Song>();
            }

            Console.WriteLine("GetOnePageOfSongsByLanguageIdNumOfWords.GetAllSongsIQueryable");
            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            if (numOfWords > 0)
            {
                totalSongs = totalSongs.Where(x => (x.LanguageId == languageId) && (x.SNumWord == numOfWords));
            }
            else
            {
                totalSongs = totalSongs.Where(x => x.LanguageId == languageId);
            }
            int[] returnNumbers = GetTotalRecordsAndPages(totalSongs, pageSize);
            int totalRecords = returnNumbers[0];            
            int totalPages = returnNumbers[1];

            int pageNo = mState.CurrentPageNo;
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all songs
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
            }
            else
            {
                if (!isWebAPI)
                {
                    if (pageNo < 1)
                    {
                        pageNo = 1;
                    }
                    else if (pageNo > totalPages)
                    {
                        pageNo = totalPages;
                    }
                }
            }

            int recordNum = (pageNo - 1) * pageSize;
            if (recordNum < 0) recordNum = 0;
            List<Song> songs = totalSongs.Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        public List<Song> GetOnePageOfNewSongByLanguageId(StateOfRequest mState, int languageId, bool isWebAPI)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryableBasicFilter(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            // only take 100 songs
            totalSongs = totalSongs.Where(x => x.LanguageId == languageId).OrderByDescending(x => x.InDate).Take(100);
            totalSongs = GetSongsIQueryableAddFilter(totalSongs, mState.QueryCondition);

            int[] returnNumbers = GetTotalRecordsAndPages(totalSongs, pageSize);
            int totalRecords = returnNumbers[0];            
            int totalPages = returnNumbers[1];

            int pageNo = mState.CurrentPageNo;
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all songs
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
            }
            else
            {
                if (!isWebAPI)
                {
                    if (pageNo < 1)
                    {
                        pageNo = 1;
                    }
                    else if (pageNo > totalPages)
                    {
                        pageNo = totalPages;
                    }
                }
            }

            int recordNum = (pageNo - 1) * pageSize;
            if (recordNum < 0) recordNum = 0;
            List<Song> songs = totalSongs.ToList().Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        public List<Song> GetOnePageOfHotSongByLanguageId(StateOfRequest mState, int languageId, bool isWebAPI)
        {
            if (mState == null)
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryableBasicFilter(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            // only take 200 songs
            totalSongs = totalSongs.Where(x => x.LanguageId == languageId).OrderByDescending(x=>x.OrderNum).Take(200);
            totalSongs = GetSongsIQueryableAddFilter(totalSongs, mState.QueryCondition);

            int[] returnNumbers = GetTotalRecordsAndPages(totalSongs, pageSize);
            int totalRecords = returnNumbers[0];            
            int totalPages = returnNumbers[1];

            int pageNo = mState.CurrentPageNo;            
            if (pageNo == -1)
            {
                // get the last page
                pageNo = totalPages;
            }
            else if (pageNo == -100)
            {
                // get all songs
                pageNo = 1; // restore pageNo to 1
                pageSize = totalRecords;    // added on 2018-11-26
                totalPages = 1; //  added on 2018-11-26
            }
            else
            {
                if (!isWebAPI)
                {
                    if (pageNo < 1)
                    {
                        pageNo = 1;
                    }
                    else if (pageNo > totalPages)
                    {
                        pageNo = totalPages;
                    }
                }
            }

            int recordNum = (pageNo - 1) * pageSize;
            if (recordNum < 0) recordNum = 0;
            List<Song> songs = totalSongs.ToList().Skip(recordNum).Take(pageSize).ToList();

            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages);

            return songs;
        }

        /// <summary>
        /// Finds the one page of songs for one song.
        /// </summary>
        /// <returns>The one page of songs for one song.</returns>
        /// <param name="mState">Song state.</param>
        /// <param name="song">Song.</param>
        /// <param name="id">Identifier.</param>
        public List<Song> FindOnePageOfSongsForOneSong(StateOfRequest mState, Song song, int id)
        {
            if ((mState == null) || (song == null))
            {
                return new List<Song>();
            }
            int pageSize = mState.PageSize;
            if (pageSize <= 0)
            {
                Console.WriteLine("pageSize cannot be less than 0.");
                return new List<Song>();
            }

            IQueryable<Song> totalSongs = GetAllSongsIQueryable(mState);
            if (totalSongs == null)
            {
                return new List<Song>();
            }

            IQueryable<Song> songsTempList = null;
        
            string condition = mState.QueryCondition.Trim();
            Console.WriteLine("FindOnePageOfSongsForOneSong.condition = " + condition);

            if (id >= 0)
            {
                // There was a song selected
                songsTempList = totalSongs.Where(x => x.Id == id);
            }
            else
            {
                // No song selected
                if (condition == "")
                {
                    // order by Id
                    int song_id = song.Id;
                    songsTempList = totalSongs.Where(x => (x.Id == song_id));
                }
                else if (condition.Equals("SongNo", StringComparison.OrdinalIgnoreCase))
                {
                    string song_no = (song.SongNo ?? "").Trim();
                    songsTempList = totalSongs.Where(x => (x.SongNo ?? "").Trim().StartsWith(song_no));
                }
                else if (condition.Equals("SongNa",StringComparison.OrdinalIgnoreCase))
                {
                    string song_na = (song.SongNa ?? "").Trim();
                    songsTempList = totalSongs.Where(x => (x.SongNa ?? "").Trim().StartsWith(song_na));
                }
                else if (condition.Equals("VodNo", StringComparison.OrdinalIgnoreCase))
                {
                    string vod_no = (song.VodNo ?? "").Trim();
                    songsTempList = totalSongs.Where(x => (x.VodNo ?? "").Trim().StartsWith(vod_no));
                }
                else if (condition.Equals("LangSongNa", StringComparison.OrdinalIgnoreCase))
                {
                    string lang_no = song.Language?.LangNo ?? "";
                    if (string.IsNullOrEmpty(lang_no) )
                    {
                        lang_no = _context.Language.FirstOrDefault()?.LangNo ?? "";
                    }
                    string song_na = (song.SongNa ?? "").Trim();
                    songsTempList = totalSongs.Where(x => (x.Language != null)
                         && ((x.Language.LangNo ?? "") == lang_no)
                         && (x.SongNa ?? "").Trim().StartsWith(song_na));
                }
                else if (condition.Equals("Singer1Na", StringComparison.OrdinalIgnoreCase))
                {
                    string singer1Na = song.Singer1?.SingNa?.Trim() ?? "";
                    songsTempList = totalSongs.Where(x => (x.Singer1 != null)
                         && (x.Singer1.SingNa ?? "").Trim().StartsWith(singer1Na));
                }
                else if (condition.Equals("Singer2Na", StringComparison.OrdinalIgnoreCase))
                {
                    string singer2Na = song.Singer2?.SingNa?.Trim() ?? "";
                    songsTempList = totalSongs.Where(x => (x.Singer2 != null)
                         && (x.Singer2.SingNa ?? "").Trim().StartsWith(singer2Na));
                }
                else if (condition.Equals("NumWordsSongNa", StringComparison.OrdinalIgnoreCase))
                {
                    int? s_num_word = song.SNumWord;
                    songsTempList = totalSongs.Where(x => (x.SNumWord == s_num_word));
                }
                else
                {
                    // not inside range of roder by then return empty lsit
                    Console.WriteLine("FindOnePageOfSongsForOneSong.wrong ");
                    return new List<Song>();
                }
            }


            int[] returnNumbers = GetTotalRecordsAndPages(totalSongs, pageSize);
            int totalRecords = returnNumbers[0];            
            int totalPages = returnNumbers[1];

            bool isFound = true;            
            Song songWithIndex = songsTempList.FirstOrDefault(); // the first one found
            if (songWithIndex == null)
            {
                isFound = false;    // song that was assigned is not found
                if (totalRecords == 0)
                {
                    // Song Table is empty
                    Console.WriteLine("FindOnePageOfSongsForOneSong.totalRecords = 0 ");
                    UpdateStateOfRequest(mState, songWithIndex, mState.CurrentPageNo, pageSize, 0, 0, true);
                    // return empty list
                    return new List<Song>();
                }
                else
                {
                    // go to last page
                    songWithIndex = totalSongs.LastOrDefault();                
                    if (songWithIndex == null) {
                        Console.WriteLine("FindOnePageOfSongsForOneSong.songWithIndex = null");
                        // return empty list
                        return new List<Song>();
                    }
                }
            }

            song.CopyFrom(songWithIndex);

            // find the row number of songWithIndex
            /*        
            int tempCount = 0;
            foreach (var songVar in totalSongs)
            {
                ++tempCount;    // first row number is 1
                if (songVar.Id == songWithIndex.Id)
                {
                    break;
                }
            }
            */
            // Get the ID we are looking for
            int targetId = songWithIndex.Id; 
            // Count all songs that appear before the one with the target ID
            // We use TakeWhile or similar logic by taking the sequence up to the match
            int countBefore = totalSongs
                .Select(x => x.Id)
                .AsEnumerable() // Transitions to memory at the last possible second
                .TakeWhile(songId => songId != targetId)
                .Count();
            int tempCount = countBefore + 1;
            int pageNo = tempCount / pageSize;
            if ((pageNo * pageSize) < tempCount)
            {
                pageNo++;
            }
            int recordNum = (pageNo - 1) * pageSize;
            if (recordNum < 0) recordNum = 0;
            List<Song> songs = totalSongs.Skip(recordNum).Take(pageSize).ToList();

            if (isFound)
            {
                // found
                mState.OrgId = song.Id; // chnaged OrgId to the song id found
            }
            else
            {
                // not found, then it is last page and last record
                mState.OrgId = 0;   // no song is selected
            }
            UpdateStateOfRequest(mState, songs.FirstOrDefault(), pageNo, pageSize, totalRecords, totalPages, true);

            return songs;
        }

        /// <summary>
        /// Finds the one song by song no.
        /// </summary>
        /// <returns>The one song by song no.</returns>
        /// <param name="song_no">Song no.</param>
        public async Task<Song> FindOneSongBySongNo(string song_no)
        {
            Song song = await _context.Song.Where(x=>x.SongNo == song_no).Include(x=>x.Language)
                            .Include(x=>x.Singer1).Include(x=>x.Singer2).SingleOrDefaultAsync();

            return song;
        }

        /// <summary>
        /// Finds the one song by identifier.
        /// </summary>
        /// <returns>The one song by identifier (Song.Id).</returns>
        /// <param name="id">the id of the song.</param>
        public async Task<Song> FindOneSongById(int id)
        {
            // find a song from context
            Song song = await _context.Song.Where(x=>x.Id == id).Include(x=>x.Language)
                        .Include(x=>x.Singer1).Include(x=>x.Singer2).SingleOrDefaultAsync();

            return song;
        }

        /// <summary>
        /// Adds the one song to table.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="song">Song.</param>
        public async Task<int> AddOneSongToTable(Song song)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (song == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.SongIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(song.SongNo))
            {
                // the song no that input by user is empty
                result = ErrorCodeModel.SongNoIsEmpty;
                return result;
            }
            Song oldSong = await FindOneSongBySongNo(song.SongNo);
            if (oldSong != null)
            {
                // song_no is duplicate
                result = ErrorCodeModel.SongNoDuplicate;
                return result;
            }

            // verifying the validation for song data
            int validCode = await VerifySong(song);
            if (validCode != ErrorCodeModel.Succeeded)
            {
                // data is invalid
                result = validCode;
                return result;
            }

            using (var dbTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    _context.Add(song);
                    await _context.SaveChangesAsync();
                    dbTransaction.Commit();
                    result = ErrorCodeModel.Succeeded;
                }
                catch (DbUpdateException ex)
                {
                    string errorMsg = ex.ToString();
                    Console.WriteLine("Failed to add one song: \n" + errorMsg);
                    dbTransaction.Rollback();
                    result = ErrorCodeModel.DatabaseError;
                }
            }

            return result;
        }

        public async Task<int> UpdateOneSongVideoInfoById(int id, string videoId, string thumbnailUrl)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                return result;
            }

            Song orgSong = await FindOneSongById(id);
            if (orgSong == null)
            {
                result = ErrorCodeModel.OriginalSongNotExist;
                return result;
            }

            orgSong.NMpeg = "00";
            orgSong.MMpeg = "00";
            orgSong.VodNo = videoId;
            orgSong.Pathname = thumbnailUrl;

            try
            {
                await _context.SaveChangesAsync();
                result = ErrorCodeModel.Succeeded;
            }
            catch (DbUpdateException ex)
            {
                string msg = ex.ToString();
                Console.WriteLine("Failed to update song video info: \n" + msg);
                result = ErrorCodeModel.DatabaseError;
            }

            return result;
        }

        /// <summary>
        /// Updates the one song by identifier.
        /// </summary>
        /// <returns>Return the error code</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="song">Song.</param>
        public async Task<int> UpdateOneSongById(int id, Song song)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, id of song cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }
            if (song == null)
            {
                // the data for updating is empty
                result = ErrorCodeModel.SongIsNull;
                return result;
            }
            if (string.IsNullOrEmpty(song.SongNo))
            {
                // the song no that input by user is empty
                result = ErrorCodeModel.SongNoIsEmpty;
                return result;
            }
            Song newSong = await FindOneSongBySongNo(song.SongNo);
            if (newSong != null)
            {
                if (newSong.Id != id)
                {
                    // song no is duplicate
                    result = ErrorCodeModel.SongNoDuplicate;
                    return result;
                }
            }

            Song orgSong = await FindOneSongById(id);
            if (orgSong == null)
            {
                // the original song does not exist any more
                result = ErrorCodeModel.OriginalSongNotExist;
                return result;
            }
            else
            {
                orgSong.CopyColumnsFrom(song);
                
                // verifying the validation for Song data
                int validCode = await VerifySong(orgSong);
                if (validCode != ErrorCodeModel.Succeeded)
                {
                    // data is invalid
                    result = validCode;
                    return result;
                }
                
                // check if entry state changed
                if ( (_context.Entry(orgSong).State) == EntityState.Modified)
                {
                    using (var dbTransaction = _context.Database.BeginTransaction())
                    {
                        try 
                        {
                            await _context.SaveChangesAsync();
                            dbTransaction.Commit();
                            result = ErrorCodeModel.Succeeded; // succeeded to update
                        }
                        catch (DbUpdateException ex)
                        {
                            string msg = ex.ToString();
                            Console.WriteLine("Failed to update song table: \n" + msg);
                            dbTransaction.Rollback();
                            result = ErrorCodeModel.DatabaseError;
                        }
                    }
                }
                else
                {
                    result = ErrorCodeModel.SongNotChanged; // no changed
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the one song by song no.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="song_no">Song no.</param>
        public async Task<int> DeleteOneSongBySongNo(string song_no)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (string.IsNullOrEmpty(song_no))
            {
                // its a bug, the original song no is empty
                result = ErrorCodeModel.OriginalSongNoIsEmpty;
                return result;
            }

            Song orgSong = await FindOneSongBySongNo(song_no);
            if (orgSong == null)
            {
                // the original song does not exist any more
                result = ErrorCodeModel.OriginalSongNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Song.Remove(orgSong);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one song. Please see log file.\n" + msg);
                        dbTransaction.Rollback();
                        result = ErrorCodeModel.DatabaseError;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the one song by identifier.
        /// </summary>
        /// <returns>Return the error code.</returns>
        /// <param name="id">Identifier.</param>
        public async Task<int> DeleteOneSongById(int id)
        {
            int result = ErrorCodeModel.ErrorBecauseBugs;
            if (id == 0)
            {
                // its a bug, the id of song cannot be 0
                result = ErrorCodeModel.ErrorBecauseBugs;
                return result;
            }

            Song orgSong = await FindOneSongById(id);
            if (orgSong == null)
            {
                // the original song does not exist any more
                result = ErrorCodeModel.OriginalSongNotExist;
            }
            else
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        _context.Song.Remove(orgSong);
                        await _context.SaveChangesAsync();
                        dbTransaction.Commit();
                        result = ErrorCodeModel.Succeeded; // succeeded to update
                    }
                    catch (DbUpdateException ex)
                    {
                        string msg = ex.ToString();
                        Console.WriteLine("Failed to delete one song. Please see log file.\n" + msg);
                        dbTransaction.Rollback();
                        result = ErrorCodeModel.DatabaseError;
                    }
                }
            }

            return result;
        }

        // end of public methods


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SongManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

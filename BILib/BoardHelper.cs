using DBModel;
using DBModel.SqlModels;
using DtoModel;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace BILib
{
    public class BoardHelper
    {
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private readonly MWDBContext webDB;

        public BoardHelper(MWDBContext context)
        {
            webDB = context;
        }


        #region 

        public IEnumerable<BoardDto> GetPopularBoards()
        {
            const string URL = @"https://www.ptt.cc/bbs/hotboards.html";
            log.Info(MethodBase.GetCurrentMethod().Name);

            try
            {
                var html = new MyWebClient().DownloadString(URL);
                var doc = new HtmlDocument(); doc.LoadHtml(HttpUtility.HtmlDecode(html));
                var sequence = 1;

                return doc.DocumentNode.SelectNodes("//a[@class='board']").Select(node => new BoardDto()
                {
                    Sn = sequence++,
                    Name = node.SelectSingleNode("div[@class='board-name']")?.InnerText,
                    BoardType = BoardType.Board,
                    OnlineCount = int.Parse(node.SelectSingleNode("div/span")?.InnerText),
                    Title = node.SelectSingleNode("div[@class='board-title']")?.InnerText,

                });
            }
            catch (Exception ex)
            {
                log.Debug($"{MethodBase.GetCurrentMethod().Name}[Exception]：{ex.Message}");
                log.Debug(ex.StackTrace);
            }

            return Enumerable.Empty<BoardDto>();
        }
        public APagePosts GetNewPostListByBoardName(string boardName, int page = 1)
        {
            var boardInfo = webDB.BoardInfo.SingleOrDefault(p => p.Board == boardName);
            if (boardInfo == null) return null;

            var pagePosts = new APagePosts
            {
                BoardInfo = new Board
                {
                    Name = boardInfo.Board,
                    Href = $"/bbs/{boardInfo.Board}/index.html",
                    Nuser = boardInfo.OnlineUser.ToString(),
                    Class = "",
                    Title = boardInfo.ChineseDes,
                    MaxSize = GetPageSize(boardName)
                }
            };

            pagePosts.Page = page;
            log.Info($"{MethodBase.GetCurrentMethod().Name}[boardName]:{boardName}");
            string text = string.Empty;
            string strPage = GetPageNo(page, pagePosts.BoardInfo.MaxSize);

            string url = $"https://www.ptt.cc/bbs/{boardName}/index{strPage}.html";
            MyWebClient client = new MyWebClient();
            client.Encoding = Encoding.UTF8; // 設定Webclient.Encoding

            string html = "未知";
            try
            {
                html = client.DownloadString(url);

                Dictionary<int, APost> dicRow = new Dictionary<int, APost>();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlDocument doctag = new HtmlDocument();
                int i = 1;
                int iwide = 0;
                var metaTags = doc.DocumentNode.SelectNodes("//div");
                if (metaTags != null)
                {
                    foreach (var tag in metaTags)
                    {

                        if (tag.Attributes["class"] != null)
                        {
                            if (tag.Attributes["class"].Value == "btn-group btn-group-paging")
                            {
                                doctag.LoadHtml(tag.OuterHtml);
                                HtmlNodeCollection htmlNodes = doctag.DocumentNode.SelectNodes("//a[@class='btn wide']");
                                foreach (var N in htmlNodes)
                                {
                                    iwide = N.OuterHtml.IndexOf("/index");
                                    if (iwide > 0)
                                    {
                                        text = N.OuterHtml.Substring(iwide, N.OuterHtml.Length - iwide);
                                        text = Regex.Replace(text, "[^0-9]", ""); //僅保留數字
                                        int.TryParse(text, out iwide);
                                        if (pagePosts.BoardInfo.MaxSize < iwide)
                                            pagePosts.BoardInfo.MaxSize = iwide;
                                    }
                                }


                            }

                            if (tag.Attributes["class"].Value == "r-ent")
                            {
                                ////class="title" class="author" class="date" 
                                doctag.LoadHtml(tag.OuterHtml);
                                APost row = new APost();
                                HtmlNode bodyNodes = doctag.DocumentNode.SelectSingleNode("//div[@class='title']/a[@href]");
                                if (bodyNodes == null) continue;
                                row.Href = bodyNodes.Attributes["href"].Value;
                                row.Title = bodyNodes.InnerText;
                                HtmlNode authorNode = doctag.DocumentNode.SelectSingleNode("//div[@class='author']");
                                row.Author = authorNode.InnerText;
                                HtmlNode dateNode = doctag.DocumentNode.SelectSingleNode("//div[@class='date']");
                                row.Date = dateNode.InnerText;
                                //--AidConverter
                                row.AID = AidConverter.urlToAid(row.Href);
                                row.Board = boardName;
                                List<PostRank> postRanks = webDB.PostRank.AsNoTracking().Where(x => x.Board == boardName && x.Aid == row.AID).ToList();
                                if (postRanks.Count > 0)
                                {
                                    row.Goup = postRanks.Count(x => x.Rank == 1);
                                    row.Down = postRanks.Count(x => x.Rank == -1);
                                }
                                //--
                                dicRow.Add(i, row);
                                i++;
                            }
                            else if (tag.Attributes["class"].Value == "r-list-sep")
                            {
                                break;
                            }


                        }


                    }

                }

                pagePosts.PostList = dicRow.Values.ToList();

            }
            catch (Exception ex)
            {
                pagePosts.Message = new Message
                {
                    Error = $"{ex.Message}.{ex.StackTrace}"
                };

                log.Debug($"GetNewPostListByBoardName[Exception]：{ex.Message}");
                log.Debug(ex.StackTrace);
            }

            return pagePosts;
        }



        public OnePost GetPost(string BoardName, string Filename)
        {
            log.Info($"GetPost[BoardName]：{BoardName}|[Filename]：{Filename}");
            string text = string.Empty;
            string url = $"https://www.ptt.cc/bbs/{BoardName}/{Filename}.html";
            MyWebClient client = new MyWebClient();
            client.Encoding = Encoding.UTF8; // 設定Webclient.Encoding

            OnePost post = new OnePost();
            post.Href = url;
            post.Board = BoardName;
            string html = "未知";
            try
            {
                html = client.DownloadString(url);

                List<string> iteamList = new List<string>();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlDocument doctag = new HtmlDocument();
                var metaTags = doc.DocumentNode.SelectNodes("//div[@id='main-content']");
                if (metaTags != null)
                {
                    foreach (var tag in metaTags)
                    {

                        if (tag.Attributes["class"] != null)
                        {
                            if (tag.Attributes["class"].Value == "bbs-screen bbs-content")
                            {
                                ////class="title" class="author" class="date" 
                                doctag.LoadHtml(tag.OuterHtml);

                                HtmlNodeCollection metaline = doctag.DocumentNode.SelectNodes("//div[@class='article-metaline']");
                                foreach (var dtag in metaline)
                                {
                                    if (dtag == null) continue;

                                    switch (dtag.ChildNodes[0].InnerText)
                                    {
                                        case "作者":
                                            string strAuthor = dtag.ChildNodes[1].InnerText;
                                            string[] sA = strAuthor.Split('(');
                                            post.Author = sA[0].Trim();
                                            if (sA.Length > 1)
                                                post.Nickname = sA[1].Replace(")", "");
                                            continue;
                                        case "標題":
                                            post.Title = dtag.ChildNodes[1].InnerText;
                                            continue;
                                        case "時間":
                                            post.Date = dtag.ChildNodes[1].InnerText;
                                            continue;
                                        default:
                                            continue;
                                    }
                                }

                                post.Comments = new List<Comment>();
                                metaline = doctag.DocumentNode.SelectNodes("//div[@class='push']");
                                if (metaline != null)
                                {
                                    foreach (var dtag in metaline)
                                    {
                                        if (dtag == null) continue;

                                        Comment push = new Comment();
                                        push.Tag = dtag.ChildNodes[0].InnerText.Trim();
                                        push.Userid = dtag.ChildNodes[1].InnerText;
                                        push.Content = dtag.ChildNodes[2].InnerText;
                                        push.IPdatetime = dtag.ChildNodes[3].InnerText;
                                        post.Comments.Add(push);
                                    }
                                }
                                StringBuilder sb = new StringBuilder();
                                string[] Content = WebUtility.HtmlDecode(tag.InnerText).Split('\n');

                                foreach (var str in Content.Skip(1))
                                {
                                    if (str.IndexOf("※ 發信站") > -1) break;
                                    if (str.IndexOf("※ 編輯") > -1) break;
                                    sb.AppendLine(str);
                                }
                                post.Content = sb.ToString();

                            }

                        }
                    }
                }


                text = Newtonsoft.Json.JsonConvert.SerializeObject(post);

                //Console.WriteLine(text);

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                log.Debug($"GetPost[Exception]：{ex.Message}");
                log.Debug(ex.StackTrace);
            }

            return post;
        }


        public void SetBoardInfo()
        {
            log.Info("SetBoardInfo");
            string text = string.Empty;

            string url = $"https://www.ptt.cc/bbs/hotboards.html";
            MyWebClient client = new MyWebClient();
            client.Encoding = Encoding.UTF8; // 設定Webclient.Encoding

            string html = string.Empty;
            try
            {
                html = client.DownloadString(url);

                List<string> iteamList = new List<string>();

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                HtmlDocument doctag = new HtmlDocument();
                var metaTags = doc.DocumentNode.SelectNodes("//div");
                if (metaTags != null)
                {
                    foreach (var tag in metaTags)
                    {

                        if (tag.Attributes["class"] != null)
                        {
                            if (tag.Attributes["class"].Value == "b-ent")
                            {
                                ////class="title" class="author" class="date" 
                                doctag.LoadHtml(tag.OuterHtml);
                                Board Board = new Board();
                                HtmlNode bodyNodes = doctag.DocumentNode.SelectSingleNode("//a[@class='board']");
                                if (bodyNodes == null) continue;
                                Board.Href = bodyNodes.Attributes["href"].Value;
                                HtmlNode HDname = doctag.DocumentNode.SelectSingleNode("//div[@class='board-name']");
                                Board.Name = HDname.InnerText;
                                HtmlNode HDnuser = doctag.DocumentNode.SelectSingleNode("//div[@class='board-nuser']");
                                Board.Nuser = HDnuser.InnerText;
                                HtmlNode HDclass = doctag.DocumentNode.SelectSingleNode("//div[@class='board-class']");
                                Board.Class = HDclass.InnerText;
                                HtmlNode HDtitle = doctag.DocumentNode.SelectSingleNode("//div[@class='board-title']");
                                Board.Title = HDtitle.InnerText;

                                if (!BContainer.dicBoardInfo.ContainsKey(Board.Name))
                                    BContainer.dicBoardInfo.Add(Board.Name, Board);

                                BContainer.dicBoardInfo[Board.Name] = Board;

                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info($"SetBoardInfo[Exception]{ex.Message}");
                log.Info($"SetBoardInfo[Exception]{ex.StackTrace}");
            }

        }


        private string GetPageNo(int page, int MaxPage)
        {
            string PageNo = "";

            page = MaxPage - page + 1;
            if ((page == 1) || (page < 1))
            {
                return PageNo;
            }

            PageNo = page.ToString();
            return PageNo;
        }

        private int GetPageSize(string boardName)
        {
            string URL = $"https://www.ptt.cc/bbs/{boardName}/index.html";
            log.Info(MethodBase.GetCurrentMethod().Name);

            try
            {
                var html = new MyWebClient().DownloadString(URL);
                var doc = new HtmlDocument(); doc.LoadHtml(HttpUtility.HtmlDecode(html));
                var node = doc.DocumentNode.SelectSingleNode("//a[.='‹ 上頁']");
                var number = Regex.Replace(node.OuterHtml, "[^0-9]", "");

                return int.Parse(number) + 1;
            }
            catch (Exception ex)
            {
                log.Debug($"{MethodBase.GetCurrentMethod().Name}[Exception]：{ex.Message}");
                log.Debug(ex.StackTrace);
            }

            return 0;
        }

        public class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest WR = base.GetWebRequest(uri);
                WR.Timeout = 30 * 1000;
                Cookie cookie = new Cookie("over18", "1", "/", "www.ptt.cc");

                TryAddCookie(WR, cookie);
                return WR;
            }

            private static bool TryAddCookie(WebRequest webRequest, Cookie cookie)
            {
                HttpWebRequest httpRequest = webRequest as HttpWebRequest;
                if (httpRequest == null)
                {
                    return false;
                }

                if (httpRequest.CookieContainer == null)
                {
                    httpRequest.CookieContainer = new CookieContainer();
                }

                httpRequest.CookieContainer.Add(cookie);
                return true;
            }

        }
        #endregion



        public RankByPost GetRank(string board, string aID)
        {
            string json = string.Empty;
            RankByPost rankByPost = new RankByPost();
            try
            {
                List<PostRank> postRanks = webDB.PostRank.AsNoTracking().Where(x => x.Board == board && x.Aid == aID).ToList();

                rankByPost.Board = board;
                rankByPost.AID = aID;
                rankByPost.Goup = postRanks.Count(x => x.Rank == 1);
                rankByPost.Down = postRanks.Count(x => x.Rank == -1);
            }
            catch (Exception ex)
            {
                json = string.Format($"{ex.Message}.{ex.InnerException?.Message}.{ex.StackTrace}");
            }
            return rankByPost;
        }

        public PostRank SetRank(string board, string aID, string pTTID, int iR)
        {
            string json = string.Empty;
            PostRank postRank = null;
            try
            {
                postRank = webDB.PostRank.Where(x => x.Board == board && x.Aid == aID && x.Pttid == pTTID).FirstOrDefault();
                if (postRank is null)
                {
                    postRank = new PostRank()
                    {
                        No = Guid.NewGuid(),
                        Board = board.Trim(),
                        Aid = aID.Trim(),
                        Pttid = pTTID.Trim(),
                        Rank = iR
                    };
                    webDB.PostRank.Add(postRank);
                }
                else
                {
                    postRank.Rank = iR;
                }
                webDB.SaveChanges();
            }
            catch (Exception ex)
            {
                json = string.Format("{0}.{1}.{2}", ex.Message, ex.InnerException.Message, ex.StackTrace);
            }
            return postRank;
        }
    }

}

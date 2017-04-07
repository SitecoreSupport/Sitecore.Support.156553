using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Resources;
using Sitecore.Resources.Media;
using Sitecore.Shell;
using Sitecore.Shell.Applications.FlashUpload.Advanced;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.Support.Shell.Applications.Media.MediaFolder
{
    public class MediaFolderForm : Sitecore.Shell.Applications.Media.MediaFolder.MediaFolderForm
    {
        protected override void RenderItem(System.Web.UI.HtmlTextWriter output, Item item)
        {
            Assert.ArgumentNotNull(output, "output");
            Assert.ArgumentNotNull(item, "item");
            string text = string.Empty;
            string text2 = string.Empty;
            string text3 = string.Empty;
            if (UploadedItems.Include(item))
            {
                UploadedItems.RenewExpiration();
            }
            string src;
            int num;
            if (HasTemplate(item.Template, new ID[] { TemplateIDs.MediaFolder, TemplateIDs.Folder, TemplateIDs.Node}))
            {
                src = Themes.MapTheme("Applications/48x48/folder.png");
                num = 48;
                int num2 = UserOptions.View.ShowHiddenItems ? item.Children.Count : this.GetVisibleChildCount(item);
                text = num2 + " " + Translate.Text((num2 == 1) ? "item" : "items");
            }
            else
            {
                MediaItem mediaItem = item;
                MediaUrlOptions thumbnailOptions = MediaUrlOptions.GetThumbnailOptions(item);
                num = (MediaManager.HasMediaContent(mediaItem) ? 72 : 48);
                thumbnailOptions.Width = num;
                thumbnailOptions.Height = num;
                thumbnailOptions.UseDefaultIcon = true;
                src = MediaManager.GetMediaUrl(mediaItem, thumbnailOptions);
                MediaMetaDataFormatter metaDataFormatter = MediaManager.Config.GetMetaDataFormatter(mediaItem.Extension);
                if (metaDataFormatter != null)
                {
                    MediaMetaDataCollection metaData = mediaItem.GetMetaData();
                    MediaMetaDataCollection mediaMetaDataCollection = new MediaMetaDataCollection();
                    foreach (string current in metaData.Keys)
                    {
                        mediaMetaDataCollection[current] = System.Web.HttpUtility.HtmlEncode(metaData[current]);
                    }
                    if (text != null)
                    {
                        text = metaDataFormatter.Format(mediaMetaDataCollection, MediaMetaDataFormatterOutput.HtmlNoKeys);
                    }
                }
                MediaValidatorResult results = mediaItem.ValidateMedia();
                MediaValidatorFormatter mediaValidatorFormatter = new MediaValidatorFormatter();
                text2 = mediaValidatorFormatter.Format(results, MediaValidatorFormatterOutput.HtmlPopup);
                LinkDatabase linkDatabase = Globals.LinkDatabase;
                ItemLink[] referrers = linkDatabase.GetReferrers(item);
                if (referrers.Length > 0)
                {
                    text3 = referrers.Length + " " + Translate.Text((referrers.Length == 1) ? "usage" : "usages");
                }
            }
            Tag tag = new Tag("a");
            tag.Add("id", "I" + item.ID.ToShortID());
            tag.Add("href", "#");
            tag.Add("onclick", "javascript:scForm.getParentForm().invoke('item:load(id=" + item.ID + ")');return false");
            if (UploadedItems.Include(item))
            {
                tag.Add("class", "highlight");
            }
            tag.Start(output);
            ImageBuilder imageBuilder = new ImageBuilder
            {
                Src = src,
                Class = "scMediaIcon",
                Width = num,
                Height = num
            };
            string text4 = string.Empty;
            if (num < 72)
            {
                num = (72 - num) / 2;
                text4 = string.Format("padding:{0}px {0}px {0}px {0}px", num);
            }
            if (!string.IsNullOrEmpty(text4))
            {
                text4 = " style=\"" + text4 + "\"";
            }
            output.Write("<div class=\"scMediaBorder\"" + text4 + ">");
            output.Write(imageBuilder.ToString());
            output.Write("</div>");
            output.Write("<div class=\"scMediaTitle\">" + item.GetUIDisplayName() + "</div>");
            if (!string.IsNullOrEmpty(text))
            {
                output.Write("<div class=\"scMediaDetails\">" + text + "</div>");
            }
            if (!string.IsNullOrEmpty(text2))
            {
                output.Write("<div class=\"scMediaValidation\">" + text2 + "</div>");
            }
            if (!string.IsNullOrEmpty(text3))
            {
                output.Write("<div class=\"scMediaUsages\">" + text3 + "</div>");
            }
            tag.End(output);
        }

        private bool HasTemplate(TemplateItem currentTemplate, IEnumerable<Data.ID> templateIds)
        {
            if (currentTemplate.ID==TemplateIDs.StandardTemplate || currentTemplate.ID.IsNull || currentTemplate.ID.ToString().Equals(String.Empty))
                return false;
            else
            {
                if (templateIds.Any(x => x == currentTemplate.ID))
                    return true;
                else
                {
                    foreach (var baseTemplate in currentTemplate.BaseTemplates)
                    {
                        if (HasTemplate(baseTemplate, templateIds))
                            return true;
                    }
                    return false;
                }
            }
        }

        private int GetVisibleChildCount(Item item)
        {
            int num = 0;
            foreach (Item item2 in item.Children)
            {
                if (!item2.Appearance.Hidden)
                {
                    num++;
                }
            }
            return num;
        }


    }


}